using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;

namespace Hsenl.Network {
    public enum IOCPSocketBufferKind : byte {
        Memory,
        Array,
    }

    // - 采用IOCP的方式进行数据收发, 这是目前对于socket最优的收发方案.
    // - 无论是StartSend还是StartRecv, 只要调用一次, 都会在完成一次后, 自动进行下一次, 直到无法获得要发送(或接收)的数据缓存为止.
    //      这么做的原因是, 希望充分利用IOCP的并发速度优势, 比如, 如果使用一次一发的话, 现在在一帧里面要发10次Send, 那么可能只有第一个发出去了, 剩下的就要等下一帧, 同样的, 下
    //      一帧也是如此, 吞吐量会大大受限, 现在让IOCP内部进行循环发送, 可以实现1帧里面一百甚至上千次的吞吐量. 这也是为什么把Send()和GetSendBuffer()分开的直接原因.
    //      同时, 也可以选择直接提供数据的方式Send, 但完成后不会自动进行下一次.
    // - 可以选择用byte[]或者memory<byte>作为缓存区.
    // 
    // 注意: IOCP虽然避免了值拷贝, 但导致了数据都在一块内存上的问题, 所以我们注意不要自行修改缓冲区的数据.
    public abstract class IOCPSocket : EpibolySocket {
        public int RecvBufferSize { get; set; } = 1024;
        public int SendBufferSize { get; set; } = 1024;

        private readonly SocketAsyncEventArgs _recvEventArgs = new();
        private readonly SocketAsyncEventArgs _sendEventArgs = new();

        private bool _recvCompletedRegisted1;
        private bool _recvCompletedRegisted2;
        private bool _recvCompletedRegisted3;
        private bool _sendCompletedRegisted1;
        private bool _sendCompletedRegisted2;
        private bool _sendCompletedRegisted3;
        private bool _isSending;
        private bool _sustainSend; // 当为true时, send完成时如果成功的话, 会继续尝试进行下一次send
        private bool _sustainSendTo; // 当为true时, send完成时如果成功的话, 会继续尝试进行下一次sendTo
        private IOCPSocketBufferKind _bufferKind; // 有时候如果对于memory支持不友好的环境时, 可以使用array. 例如unity中, 对于memory的支持就不友好

        public bool IsSending => this._isSending;
        public IOCPSocketBufferKind BufferKind => this._bufferKind;

        protected IOCPSocket(Socket socket, IOCPSocketBufferKind bufferKind = IOCPSocketBufferKind.Memory) : base(socket) {
            this._bufferKind = bufferKind;
        }

        // 以下几种接收方案都能接收到数据, 不局限于发送方使用的是那种发送方案
        protected void StartRecvAsync_Internal() {
            this.CheckDisposedException();

            if (!this._recvCompletedRegisted1) {
                this._recvCompletedRegisted1 = true;
                this._recvEventArgs.Completed += this.RecvEventArgs_OnCompleted;
            }

            this.ReceiveAsync(this._recvEventArgs);
        }

        // 接收的同时, 能够拿到对方的地址
        protected void StartRecvFromAsync_Internal() {
            this.CheckDisposedException();

            if (!this._recvCompletedRegisted2) {
                this._recvCompletedRegisted2 = true;
                // 随便设置一个无效端点, 不空就行, 后续接收到数据时, 该值会被系统替换为目标端点
                this._recvEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.None, 0);
                this._recvEventArgs.Completed += this.RecvFromEventArgs_Completed;
            }

            this.ReceiveFromAsync(this._recvEventArgs);
        }

        // 不仅能拿到地址, 还能拿到IP头部信息和SocketFlags
        protected void StartRecvMessageFromAsync_Internal() {
            this.CheckDisposedException();

            if (!this._recvCompletedRegisted3) {
                this._recvCompletedRegisted3 = true;
                this._recvEventArgs.RemoteEndPoint = new IPEndPoint(IPAddress.None, 0);
                this._recvEventArgs.Completed += this.RecvMessageFromEventArgs_Completed;
            }

            this.ReceiveMessageFromAsync(this._recvEventArgs);
        }

        // tcp(不能指定地址, 必须依赖一个已经连接的socket来发送)
        protected void StartSendAsync_Internal(SocketFlags socketFlags = SocketFlags.None) {
            this.CheckDisposedException();

            if (!this._sendCompletedRegisted1) {
                this._sendCompletedRegisted1 = true;
                this._sendEventArgs.Completed += this.SendEventArgs_OnCompleted;
            }

            this._sendEventArgs.SocketFlags = socketFlags;
            this._sustainSend = true;
            this.SendAsync(this._sendEventArgs);
        }

        protected void SendAsync_Internal(byte[] data, int offset, int count, SocketFlags socketFlags = SocketFlags.None) {
            if (count == 0)
                return;

            this.CheckDisposedException();

            if (!this._sendCompletedRegisted1) {
                this._sendCompletedRegisted1 = true;
                this._sendEventArgs.Completed += this.SendEventArgs_OnCompleted;
            }

            this.CheckGreaterThanSendSizeException(count);
            this._sendEventArgs.SetBuffer(data, offset, count);
            this._sendEventArgs.SocketFlags = socketFlags;
            this.SendAsync(this._sendEventArgs, false);
        }

        // tcp
        // 适合需要高效发送多个缓冲区或文件数据的场景
        protected void SendPacketsAsync_Internal(IList<ArraySegment<byte>> buffers, SocketFlags socketFlags = SocketFlags.None) {
            this.CheckDisposedException();

            if (!this._sendCompletedRegisted2) {
                this._sendCompletedRegisted2 = true;
                this._sendEventArgs.Completed += this.SendPacketsEventArgs_OnCompleted;
            }

            this._sendEventArgs.BufferList = buffers;
            this._sendEventArgs.SocketFlags = socketFlags;
            this.SendPacketsAsync(this._sendEventArgs);
        }

        // udp(发送必须指定一个地址)
        protected void StartSendToAsync_Internal(SocketFlags socketFlags = SocketFlags.None) {
            this.CheckDisposedException();

            if (!this._sendCompletedRegisted3) {
                this._sendCompletedRegisted3 = true;
                this._sendEventArgs.Completed += this.SendToEventArgs_OnCompleted;
            }

            this._sendEventArgs.SocketFlags = socketFlags;
            this._sustainSendTo = true;
            this.SendToAsync(this._sendEventArgs);
        }

        protected void SendToAsync_Internal(byte[] data, int offset, int count, EndPoint remoteEndPoint, SocketFlags socketFlags = SocketFlags.None) {
            if (count == 0)
                return;

            this.CheckDisposedException();

            if (!this._sendCompletedRegisted3) {
                this._sendCompletedRegisted3 = true;
                this._sendEventArgs.Completed += this.SendToEventArgs_OnCompleted;
            }

            this.CheckGreaterThanSendSizeException(count);
            this._sendEventArgs.SetBuffer(data, offset, count);
            this._sendEventArgs.RemoteEndPoint = remoteEndPoint;
            this._sendEventArgs.SocketFlags = socketFlags;
            this.SendToAsync(this._sendEventArgs, false);
        }

        // 接收数据
        private void ReceiveAsync(SocketAsyncEventArgs e) {
            switch (this._bufferKind) {
                case IOCPSocketBufferKind.Memory:
                    var memory = this.GetRecvBuffer(this.RecvBufferSize);
                    this.CheckGreaterThanRecvSizeException(memory.Length);
                    e.SetBuffer(memory);
                    break;
                case IOCPSocketBufferKind.Array:
                    var bytes = this.GetRecvBuffer(this.RecvBufferSize, out int offset, out int count);
                    this.CheckGreaterThanRecvSizeException(count);
                    e.SetBuffer(bytes, offset, count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!this.Socket.ReceiveAsync(e)) {
                this.ProcessRecv(e);
            }
        }

        private void RecvEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Receive)
                return;

            this.ProcessRecv(e);
        }

        private void ProcessRecv(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                // 当关闭socket的时候, 假如有未处理完的异步操作, 则socket会立即执行一次回调, 把这次回调排除在外
                // 由于回调来自不同的线程, 所以仅靠赋值为空等判断方式并不能保证一定准确, 而如果加锁的话, 又会影响EventArgs的接收效率, 所以使用BytesTransferred判断
                // 是一种较为稳妥的方式
                if (e.BytesTransferred != 0) {
                    switch (this._bufferKind) {
                        case IOCPSocketBufferKind.Memory:
                            var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);
                            try {
                                this.OnRecv(data);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        case IOCPSocketBufferKind.Array:
                            try {
                                this.OnRecv(e.Buffer, e.Offset, e.BytesTransferred);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                this.ReceiveAsync(e);
            }
            else {
                this.Error((int)e.SocketError);
            }
        }

        // 发送数据
        private void SendAsync(SocketAsyncEventArgs e, bool getBufferByExternal = true) {
            if (getBufferByExternal) {
                switch (this._bufferKind) {
                    case IOCPSocketBufferKind.Memory:
                        var buffer = this.GetSendBuffer(this.SendBufferSize);
                        if (buffer.Length == 0) {
                            this._isSending = false;
                            return;
                        }

                        this.CheckGreaterThanSendSizeException(buffer.Length);
                        e.SetBuffer(buffer);
                        break;
                    case IOCPSocketBufferKind.Array:
                        var bytes = this.GetSendBuffer(this.SendBufferSize, out int offset, out int count);
                        if (count == 0 || bytes == null) {
                            this._isSending = false;
                            return;
                        }

                        this.CheckGreaterThanSendSizeException(count);
                        e.SetBuffer(bytes, offset, count);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this._isSending = true;

            if (!this.Socket.SendAsync(e)) {
                this.ProcessSend(e);
            }
        }

        private void SendEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Send)
                return;

            this.ProcessSend(e);
        }

        private void ProcessSend(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (e.BytesTransferred != 0) {
                    switch (this._bufferKind) {
                        case IOCPSocketBufferKind.Memory:
                            var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);
                            try {
                                this.OnSend(data);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        case IOCPSocketBufferKind.Array:
                            try {
                                this.OnSend(e.Buffer, e.Offset, e.BytesTransferred);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (this._sustainSend)
                    this.SendAsync(e);
                else
                    this._isSending = false;
            }
            else {
                this._isSending = false;
                this.Error((int)e.SocketError);
            }
        }

        // 发送数据packets
        private void SendPacketsAsync(SocketAsyncEventArgs e) {
            this._isSending = true;
            if (!this.Socket.SendPacketsAsync(e)) {
                this.ProcessSendPackets(e);
            }
        }

        private void SendPacketsEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.SendPackets)
                return;

            this.ProcessSendPackets(e);
        }

        private void ProcessSendPackets(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (e.BytesTransferred == 0)
                    return;
            }
            else {
                this._isSending = false;
                this.Error((int)e.SocketError);
            }
        }

        // 接收数据from
        private void ReceiveFromAsync(SocketAsyncEventArgs e) {
            switch (this._bufferKind) {
                case IOCPSocketBufferKind.Memory:
                    var memory = this.GetRecvBuffer(this.RecvBufferSize);
                    this.CheckGreaterThanRecvSizeException(memory.Length);
                    e.SetBuffer(memory);
                    break;
                case IOCPSocketBufferKind.Array:
                    var bytes = this.GetRecvBuffer(this.RecvBufferSize, out int offset, out int count);
                    this.CheckGreaterThanRecvSizeException(count);
                    e.SetBuffer(bytes, offset, count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!this.Socket.ReceiveFromAsync(e)) {
                this.ProcessRecvFrom(e);
            }
        }

        private void RecvFromEventArgs_Completed(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.ReceiveFrom)
                return;

            this.ProcessRecvFrom(e);
        }

        private void ProcessRecvFrom(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (e.BytesTransferred != 0) {
                    switch (this._bufferKind) {
                        case IOCPSocketBufferKind.Memory:
                            var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);
                            try {
                                this.OnRecvFrom(e.RemoteEndPoint, data);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        case IOCPSocketBufferKind.Array:
                            try {
                                this.OnRecvFrom(e.RemoteEndPoint, e.Buffer, e.Offset, e.BytesTransferred);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                this.ReceiveFromAsync(e);
            }
            else {
                this.Error((int)e.SocketError);
            }
        }

        // 发送数据to
        private void SendToAsync(SocketAsyncEventArgs e, bool getBufferByExternal = true) {
            if (getBufferByExternal) {
                switch (this._bufferKind) {
                    case IOCPSocketBufferKind.Memory: {
                        var buffer = this.GetSendToBuffer(this.SendBufferSize, out var remoteEndPoint);
                        if (buffer.Length == 0) {
                            this._isSending = false;
                            return;
                        }

                        this.CheckGreaterThanSendSizeException(buffer.Length);
                        e.RemoteEndPoint = remoteEndPoint;
                        e.SetBuffer(buffer);
                        break;
                    }
                    case IOCPSocketBufferKind.Array: {
                        var bytes = this.GetSendToBuffer(this.SendBufferSize, out int offset, out int count, out var remoteEndPoint);
                        if (count == 0 || bytes == null) {
                            this._isSending = false;
                            return;
                        }

                        this.CheckGreaterThanSendSizeException(count);
                        e.RemoteEndPoint = remoteEndPoint;
                        e.SetBuffer(bytes, offset, count);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            this._isSending = true;

            if (!this.Socket.SendToAsync(e)) {
                this.ProcessSendTo(e);
            }
        }

        private void SendToEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.SendTo)
                return;

            this.ProcessSendTo(e);
        }

        private void ProcessSendTo(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (e.BytesTransferred != 0) {
                    switch (this._bufferKind) {
                        case IOCPSocketBufferKind.Memory:
                            var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);
                            try {
                                this.OnSendTo(e.RemoteEndPoint, data);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        case IOCPSocketBufferKind.Array:
                            try {
                                this.OnSendTo(e.RemoteEndPoint, e.Buffer, e.Offset, e.BytesTransferred);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                if (this._sustainSendTo)
                    this.SendToAsync(e);
                else
                    this._isSending = false;
            }
            else {
                this._isSending = false;
                this.Error((int)e.SocketError);
            }
        }

        // 接收数据message from
        private void ReceiveMessageFromAsync(SocketAsyncEventArgs e) {
            switch (this._bufferKind) {
                case IOCPSocketBufferKind.Memory:
                    var memory = this.GetRecvBuffer(this.RecvBufferSize);
                    this.CheckGreaterThanRecvSizeException(memory.Length);
                    e.SetBuffer(memory);
                    break;
                case IOCPSocketBufferKind.Array:
                    var bytes = this.GetRecvBuffer(this.RecvBufferSize, out int offset, out int count);
                    this.CheckGreaterThanRecvSizeException(count);
                    e.SetBuffer(bytes, offset, count);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (!this.Socket.ReceiveMessageFromAsync(e)) {
                this.ProcessRecvMessageFrom(e);
            }
        }

        private void RecvMessageFromEventArgs_Completed(object sender, SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.ReceiveMessageFrom)
                return;

            this.ProcessRecvMessageFrom(e);
        }

        private void ProcessRecvMessageFrom(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) {
                if (e.BytesTransferred != 0) {
                    switch (this._bufferKind) {
                        case IOCPSocketBufferKind.Memory:
                            var data = e.MemoryBuffer.Slice(0, e.BytesTransferred);
                            try {
                                this.OnRecvMessageFrom(e.RemoteEndPoint, e.ReceiveMessageFromPacketInfo, e.SocketFlags, data);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        case IOCPSocketBufferKind.Array:
                            try {
                                this.OnRecvMessageFrom(e.RemoteEndPoint, e.ReceiveMessageFromPacketInfo, e.SocketFlags, e.Buffer, e.Offset, e.BytesTransferred);
                            }
                            catch (Exception exception) {
                                Log.Error(exception);
                            }

                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                this.ReceiveMessageFromAsync(e);
            }
            else {
                this.Error((int)e.SocketError);
            }
        }

        protected abstract Memory<byte> GetRecvBuffer(int length);
        protected abstract Memory<byte> GetSendBuffer(int length);
        protected abstract Memory<byte> GetSendToBuffer(int length, out EndPoint remoteEndPoint);
        protected abstract byte[] GetRecvBuffer(int length, out int offset, out int count);
        protected abstract byte[] GetSendBuffer(int length, out int offset, out int count);
        protected abstract byte[] GetSendToBuffer(int length, out int offset, out int count, out EndPoint remoteEndPoint);

        protected virtual void OnRecv(Memory<byte> data) { }
        protected virtual void OnRecv(byte[] data, int offset, int count) { }

        protected virtual void OnRecvFrom(EndPoint remoteEndPoint, Memory<byte> data) { }
        protected virtual void OnRecvFrom(EndPoint remoteEndPoint, byte[] data, int offset, int count) { }

        protected virtual void OnRecvMessageFrom(EndPoint remoteEndPoint, IPPacketInformation information, SocketFlags socketFlags, Memory<byte> data) { }

        protected virtual void OnRecvMessageFrom(EndPoint remoteEndPoint, IPPacketInformation information, SocketFlags socketFlags, byte[] data, int offset,
            int count) { }

        protected virtual void OnSend(Memory<byte> data) { }
        protected virtual void OnSend(byte[] data, int offset, int count) { }

        protected virtual void OnSendTo(EndPoint remoteEndPoint, Memory<byte> data) { }
        protected virtual void OnSendTo(EndPoint remoteEndPoint, byte[] data, int offset, int count) { }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();

            this.RecvBufferSize = 0;
            this.SendBufferSize = 0;

            this._recvEventArgs.Completed -= this.RecvEventArgs_OnCompleted;
            this._recvEventArgs.Completed -= this.RecvFromEventArgs_Completed;
            this._recvEventArgs.Completed -= this.RecvMessageFromEventArgs_Completed;
            this._recvEventArgs.RemoteEndPoint = null;

            this._sendEventArgs.Completed -= this.SendEventArgs_OnCompleted;
            this._sendEventArgs.Completed -= this.SendPacketsEventArgs_OnCompleted;
            this._sendEventArgs.Completed -= this.SendToEventArgs_OnCompleted;
            this._sendEventArgs.RemoteEndPoint = null;
            this._sendEventArgs.BufferList = null;

            this._recvCompletedRegisted1 = false;
            this._recvCompletedRegisted2 = false;
            this._recvCompletedRegisted3 = false;
            this._sendCompletedRegisted1 = false;
            this._sendCompletedRegisted2 = false;
            this._sendCompletedRegisted3 = false;

            this._isSending = false;
            this._sustainSend = false;
            this._sustainSendTo = false;
            this._bufferKind = IOCPSocketBufferKind.Memory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckGreaterThanRecvSizeException(int len) {
            if (len > this.RecvBufferSize)
                Log.Error($"RecvBufferLength over maximum '{this.RecvBufferSize}'");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CheckGreaterThanSendSizeException(int len) {
            if (len > this.SendBufferSize)
                Log.Error($"SendBufferLength over maximum '{this.SendBufferSize}'");
        }
    }
}