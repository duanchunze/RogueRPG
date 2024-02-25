using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace Hsenl.Network {
    public class IOCPChannel {
        protected SocketAsyncEventArgs recvEventArgs;
        protected SocketAsyncEventArgs sendEventArgs;

        protected PacketBuffer recvBuffer;
        protected PacketBuffer sendBufferSending; // 两个send缓存区, 一个用来发送, 一个用来写入, 谁没在发送中, 谁当写入
        protected PacketBuffer sendBufferWriting;
        private volatile bool _isSending;

        public bool IsSending {
            get => this._isSending;
            protected set => this._isSending = value;
        }

        protected int _recvBufferCapacity; // 消息接收缓冲区的大小
        protected int _maximumMessageBodySize = 0; // 允许用户发送的最大包体大小
        protected int _maxinumSendSizeOnce; // 一次发送的最大消息大小

        protected int totalMessageSize; // 消息总大小, 如果该值不为空, 则代表当前有消息没接收完整
        protected int alreadReadSize; // 已经接收了多少
        protected PacketBuffer incompleteRecvCache; // 已经接收的暂时缓存在这里(如果接收的包大小没有超出接收区容量, 则不会使用cache)

        protected bool closed;

        public event Action<IOCPChannel, Memory<byte>> OnRecvDataIncludeIncomplete; // 只要收到消息就通知, 不管完整不完整, 或者是否是多个消息
        public event Action<IOCPChannel, Memory<byte>> OnRecvData; // 只当一个完整的包成型时, 才通知
        public event Action<IOCPChannel, SocketAsyncEventArgs> OnError;

        private readonly object _locker = new();

        // 拿到这个buffer, 填充要发送的数据, 但不用着急马上Send, 可以等到帧末调用一次Send, 这样既不会等待一帧的延迟, 同时也可以避免频繁的send, 占用通道.
        // 写入时机没有限制, 任意时机写都行, 加了锁就可以在多线程里写
        public PacketBuffer SendBuffer {
            get {
                var sendBuffer = this.sendBufferWriting;

                if (sendBuffer.Position > 1048576) {
                    Log.Warning($"send write buffer too large! '{this.sendBufferWriting.Position}'");
                }

                return sendBuffer;
            }
        }

        public void Init(int recvBufferCapacity, int maxinumSendSizeOnce, int maximumMessageBodySize = 0) {
            this._recvBufferCapacity = recvBufferCapacity;
            this._maximumMessageBodySize = maximumMessageBodySize == 0 ? recvBufferCapacity * 1024 : maximumMessageBodySize;
            this._maxinumSendSizeOnce = maxinumSendSizeOnce;
            this.recvEventArgs ??= new();
            this.sendEventArgs ??= new();
            this.recvEventArgs.Completed += this.RecvEventArgs_OnCompleted;
            this.sendEventArgs.Completed += this.SendEventArgs_OnCompleted;
            if (this.recvBuffer == null || this.recvBuffer.Capacity != recvBufferCapacity) {
                this.recvBuffer = new PacketBuffer(recvBufferCapacity);
            }

            this.sendBufferSending ??= new PacketBuffer();
            this.sendBufferWriting ??= new PacketBuffer();
            this.totalMessageSize = 0;
            this.alreadReadSize = 0;
            this.incompleteRecvCache ??= new PacketBuffer();

            this.closed = false;
        }

        // 启动
        public void Start(Socket socket) {
            this.recvEventArgs.UserToken = socket;
            this.sendEventArgs.UserToken = socket;

            this.ReceiveAsync(this.recvEventArgs);
        }

        // 接收数据
        private void ReceiveAsync(SocketAsyncEventArgs e) {
            var start = 0;
            var len = this.recvBuffer.Capacity;
            e.SetBuffer(this.recvBuffer.AsMemory(start, len));
            var socket = (Socket)e.UserToken;
            if (!socket.ReceiveAsync(e)) {
                Log.Info("同步接收数据了...");
                this.ProcessRecv(e);
            }
        }

        private void RecvEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Receive)
                    return;

                lock (this._locker) {
                    this.ProcessRecv(e);
                }
            }
            catch (Exception exception) {
                Log.Error(exception);
                this.Error(e);
            }
        }

        private void ProcessRecv(SocketAsyncEventArgs e) {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) {
                try {
                    this.OnRecvDataIncludeIncomplete?.Invoke(this, e.MemoryBuffer.Slice(0, e.BytesTransferred));
                }
                catch (Exception exception) {
                    Log.Error(exception);
                }

                this.ProcessRecvData(e.MemoryBuffer.Slice(0, e.BytesTransferred));
                this.ReceiveAsync(e);
            }
            else {
                this.Error(e);
            }
        }

        private void ProcessRecvData(Memory<byte> data) {
            while (true) {
                var len = data.Length;
                if (this.totalMessageSize == 0 && this.alreadReadSize == 0) {
                    if (len <= 8) {
                        Log.Error($"Message is fragmentary {len}");
                        return;
                    }

                    // 能到这里, 说明之前的包已经读完了, 这是一个新的包
                    // 得到这个包体总共多大
                    var totalBodySizeMemory = data.Slice(0, Constant.MessageHeadFirstHalfSize);
                    var totalBodySize = Unsafe.ReadUnaligned<int>(ref MemoryMarshal.GetReference(totalBodySizeMemory.Span));
                    if (totalBodySize <= 0) {
                        Log.Error($"Invalid Head Message Size '{totalBodySize}'");
                        return;
                    }

                    // 算出现在拿到的消息体的长度
                    var haveBodyLen = len - Constant.MessageHeadSize;
                    if (totalBodySize > haveBodyLen) {
                        // 说明当前包没接收完, 把数据暂时缓存, 待读完再发送出去
                        this.totalMessageSize = totalBodySize;
                        this.alreadReadSize = haveBodyLen;
                        data.CopyTo(this.incompleteRecvCache.GetMemory(len));
                        this.incompleteRecvCache.Advance(len);
                    }
                    else {
                        // 读完了, 抛出事件
                        int completeMessageSize = totalBodySize + Constant.MessageHeadSize;
                        try {
                            this.OnRecvData?.Invoke(this, data.Slice(0, completeMessageSize));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }

                        // 还有剩余的数据, 说明剩余的数据流里面还含有新的包, 再次处理
                        if (len > completeMessageSize) {
                            data = data.Slice(completeMessageSize);
                            continue;
                        }
                    }
                }
                else {
                    // 之前的包没读完, 继续读之前的包
                    if (this.alreadReadSize + len < this.totalMessageSize) {
                        this.alreadReadSize += len;
                        // 还是没读完, 继续缓存
                        data.CopyTo(this.incompleteRecvCache.GetMemory(len));
                        this.incompleteRecvCache.Advance(len);
                    }
                    else {
                        // 之前的包读完了, 抛出事件
                        int lacking = 0;
                        try {
                            // 算出还缺多少数据
                            lacking = this.totalMessageSize - this.alreadReadSize;
                            data.Slice(0, lacking).CopyTo(this.incompleteRecvCache.GetMemory(lacking));
                            this.incompleteRecvCache.Advance(lacking);
                            this.OnRecvData?.Invoke(this, this.incompleteRecvCache.AsMemory(0, (int)this.incompleteRecvCache.Position));
                        }
                        catch (Exception exception) {
                            Log.Error(exception);
                        }
                        finally {
                            this.totalMessageSize = 0;
                            this.alreadReadSize = 0;
                            this.incompleteRecvCache.Seek(0, SeekOrigin.Begin);
                        }

                        // 还有剩余的数据, 则再次处理
                        if (len > lacking) {
                            data = data.Slice(lacking);
                            continue;
                        }
                    }
                }

                break;
            }
        }

        // 发送数据
        private void SendAsync(SocketAsyncEventArgs e) {
            if (!this.sendBufferSending.HasData)
                return;

            this.IsSending = true;
            // 计算还需要发送多少
            var len = (int)this.sendBufferSending.Position - this.sendBufferSending.Origin;
            if (len > this._maxinumSendSizeOnce) {
                len = this._maxinumSendSizeOnce;
            }

            e.SetBuffer(this.sendBufferSending.AsMemory(0, len));
            this.sendBufferSending.Origin += len;

            var socket = (Socket)e.UserToken;
            if (!socket.SendAsync(e)) {
                Log.Error("456");
                this.ProcessSend(e);
            }
        }

        private void SendEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Send)
                    return;

                lock (this._locker) {
                    this.ProcessSend(e);
                }
            }
            catch (Exception exception) {
                Log.Error(exception);
                this.Error(e);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (this.sendBufferSending.Origin == 0 || !this.sendBufferSending.HasData) {
                    // 一次就发送完了, 或者是反复发送完了, 重置自己
                    this.IsSending = false;
                    this.sendBufferSending.Origin = 0;
                    this.sendBufferSending.Seek(0, SeekOrigin.Begin);
                }
                else {
                    // 如果没发送完, 则继续发
                    this.SendAsync(e);
                }
            }
            else {
                this.IsSending = false;
                this.Error(e);
            }
        }

        private void Error(SocketAsyncEventArgs e) {
            try {
                this.OnError?.Invoke(this, e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public bool Send() {
            if (this.IsSending)
                return false;

            // 把sending替换为我们的writing
            ObjectHelper.Swap(ref this.sendBufferSending, ref this.sendBufferWriting);

            this.SendAsync(this.sendEventArgs);
            return true;
        }

        public virtual void Close() {
            if (this.closed)
                return;

            this.closed = true;

            this.recvEventArgs.Completed -= this.RecvEventArgs_OnCompleted;
            this.sendEventArgs.Completed -= this.SendEventArgs_OnCompleted;
            var socket = (Socket)this.recvEventArgs.UserToken;
            socket.Close();
            this.recvEventArgs.UserToken = null;
            this.sendEventArgs.UserToken = null;
            this.recvBuffer.Seek(0, SeekOrigin.Begin);
            this.recvBuffer.Reset();
            this.sendBufferSending.Seek(0, SeekOrigin.Begin);
            this.sendBufferSending.Reset();
            this.sendBufferWriting.Seek(0, SeekOrigin.Begin);
            this.sendBufferWriting.Reset();
            this._isSending = false;
            this._recvBufferCapacity = 0;
            this._maximumMessageBodySize = 0;
            this._maxinumSendSizeOnce = 0;
            this.totalMessageSize = 0;
            this.alreadReadSize = 0;
            this.incompleteRecvCache.Seek(0, SeekOrigin.Begin);
            this.OnRecvDataIncludeIncomplete = null;
            this.OnRecvData = null;
            this.OnError = null;
        }

        public virtual void Dispose() {
            this.Close();

            this.recvEventArgs = null;
            this.sendEventArgs = null;
            this.recvBuffer.Dispose();
            this.recvBuffer = null;
            this.sendBufferSending.Dispose();
            this.sendBufferSending = null;
            this.sendBufferWriting.Dispose();
            this.sendBufferWriting = null;
            this.incompleteRecvCache = null;
        }
    }
}