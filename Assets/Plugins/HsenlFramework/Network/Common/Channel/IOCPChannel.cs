using System;
using System.Net.Sockets;
using System.Threading;

namespace Hsenl.Network {
    // 使用IOCP技术来实现数据收发
    // todo 定个最低发送值, 如果数据太小, 就积压一下, 除非强制调用send, 或者超出最大积压时间, 交给一个专门的插件去处理
    public class IOCPChannel : Channel {
        private SocketAsyncEventArgs _recvEventArgs = new();
        private SocketAsyncEventArgs _sendEventArgs = new();

        private IPackageRecvBufferProvider _recvBufferProvider;
        private IPackageSendBufferProvider _sendBufferProvider;
        private IPackageReader _packageReader;
        private IPackageWriter _packageWriter;

        // 启动
        public override void Start(Socket socket) {
            this.Socket = socket;
            this._recvEventArgs.UserToken = socket;
            this._sendEventArgs.UserToken = socket;
            this._recvEventArgs.Completed += this.RecvEventArgs_OnCompleted;
            this._sendEventArgs.Completed += this.SendEventArgs_OnCompleted;
            this._packageReader.OnMessageReaded = (opcode, data) => { this.OnRecvMessage?.Invoke(this.ChannelId, opcode, data); };
            this._packageWriter.OnMessageWrited = (opcode, data) => { this.OnSendMessage?.Invoke(this.ChannelId, opcode, data); };

            this.ReceiveAsync(this._recvEventArgs);
        }

        // 接收数据
        private void ReceiveAsync(SocketAsyncEventArgs e) {
            e.SetBuffer(this._recvBufferProvider.GetRecvBuffer(this.RecvBufferSize));
            var socket = (Socket)e.UserToken;
            if (!socket.ReceiveAsync(e)) {
                this.ProcessRecv(e);
            }
        }

        private void RecvEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Receive)
                    return;

                this.ProcessRecv(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
                this.Error(ErrorCode.Error_CodeException);
            }
        }

        private void ProcessRecv(SocketAsyncEventArgs e) {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) {
                Interlocked.Add(ref this.totalBytesRecv, e.BytesTransferred);
                var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);

                try {
                    this.OnRecvPackage?.Invoke(this.ChannelId, data);
                }
                catch (Exception exception) {
                    Log.Error(exception);
                }

                try {
                    this._packageReader.Read(data);
                }
                catch (Exception exception) {
                    Log.Error(exception);
                    this.Error(ErrorCode.Error_CodeException);
                }

                try {
                    this.ReceiveAsync(e);
                }
                catch (Exception exception) {
                    Log.Error(exception);
                    this.Error(ErrorCode.Error_CodeException);
                }
            }
            else {
                this.Error(ErrorCode.Error_ConnectingOutage);
            }
        }

        // 发送数据
        private void SendAsync(SocketAsyncEventArgs e) {
            Memory<byte> buffer;
            try {
                buffer = this._sendBufferProvider.GetSendBuffer(0, this.SendBufferSize);
                if (buffer.Length == 0) {
                    this.IsSending = false;
                    return;
                }

                this.IsSending = true;
            }
            catch (Exception exception) {
                Log.Error(exception);
                this.Error(ErrorCode.Error_CodeException);
                return;
            }

            try {
                this.OnSendPackage?.Invoke(this.ChannelId, buffer);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }

            e.SetBuffer(buffer);
            var socket = (Socket)e.UserToken;
            if (!socket.SendAsync(e)) {
                this.ProcessSend(e);
            }
        }

        private void SendEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Send)
                    return;

                this.ProcessSend(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
                this.Error(ErrorCode.Error_CodeException);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                Interlocked.Add(ref this.totalBytesSend, e.BytesTransferred);
                this.SendAsync(e);
            }
            else {
                this.IsSending = false;
                this.Error(ErrorCode.Error_ConnectingOutage);
            }
        }

        private void Error(int errorCode) {
            try {
                this.OnError?.Invoke(this.ChannelId, errorCode);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public override void Write(Func<PackageBuffer, ushort> func) {
            this._packageWriter.Write(func);
        }

        public override bool Send() {
            if (Socket.Poll(1, SelectMode.SelectError)) {
                Log.Error("断开了~");
            }

            if (this.IsSending)
                return false;

            this._sendBufferProvider.SendNext();

            this.SendAsync(this._sendEventArgs);
            return true;
        }

        public override void Disconnect() {
            base.Disconnect();

            this._recvEventArgs.Completed -= this.RecvEventArgs_OnCompleted;
            this._recvEventArgs.UserToken = null;
            this._sendEventArgs.Completed -= this.SendEventArgs_OnCompleted;
            this._sendEventArgs.UserToken = null;

            this._recvBufferProvider.Reset();
            this._sendBufferProvider.Reset();
            this._packageReader.Reset();
            this._packageWriter.Reset();
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;
            
            this.Disconnect();
            base.Dispose();

            this._recvEventArgs?.Dispose();
            this._recvEventArgs = null;
            this._sendEventArgs?.Dispose();
            this._sendEventArgs = null;

            this._recvBufferProvider?.Dispose();
            this._recvBufferProvider = null;
            this._sendBufferProvider?.Dispose();
            this._sendBufferProvider = null;
            this._packageReader?.Dispose();
            this._packageReader = null;
            this._packageWriter?.Dispose();
            this._packageWriter = null;
        }
    }
}