using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 使用
    // 必须提供: 1、必须提供RecvBufferGetter 2、必须提供SendBufferGetter
    // 可以获得: 1、当数据接受的事件 2、当数据被发送出去的事件
    public class TcpSocket : IOCPSocket {
        public Func<int, Memory<byte>> RecvBufferGetter { get; set; }
        public Func<int, Memory<byte>> SendBufferGetter { get; set; }
        public Action<Memory<byte>> OnRecvData { get; set; }
        public Action<Memory<byte>> OnSendData { get; set; }

        public TcpSocket(AddressFamily addressFamily) : base(new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp)) { }

        public TcpSocket(Socket socket) : base(socket) {
            if (socket.ProtocolType != ProtocolType.Tcp)
                throw new ArgumentException();

            if (socket.SocketType != SocketType.Stream)
                throw new ArgumentException();
        }

        protected override Memory<byte> GetRecvBuffer(int length) {
            return this.RecvBufferGetter(length);
        }

        protected override Memory<byte> GetSendBuffer(int length) {
            return this.SendBufferGetter(length);
        }

        protected override Memory<byte> GetSendToBuffer(int length, out EndPoint remoteEndPoint) {
            throw new NotImplementedException();
        }

        protected override byte[] GetRecvBuffer(int length, out int offset, out int count) {
            throw new NotImplementedException();
        }

        protected override byte[] GetSendBuffer(int length, out int offset, out int count) {
            throw new NotImplementedException();
        }

        protected override byte[] GetSendToBuffer(int length, out int offset, out int count, out EndPoint remoteEndPoint) {
            throw new NotImplementedException();
        }

        protected override void OnRecv(Memory<byte> data) {
            try {
                this.OnRecvData?.Invoke(data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnSend(Memory<byte> data) {
            try {
                this.OnSendData?.Invoke(data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void StartReceiveAsync() {
            this.StartRecvAsync_Internal();
        }

        public void StartSendAsync() {
            if (this.IsSending)
                return; // 已经在发了, 不需要再开始了

            this.StartSendAsync_Internal();
        }

        public bool SendPacketsAsync(IList<ArraySegment<byte>> buffers) {
            if (this.IsSending)
                return false; // 说明当前有没发完的数据正在发送, 该次send无效, 需要下次再尝试send

            this.SendPacketsAsync_Internal(buffers);
            return true;
        }
        
        public bool SendAsync(byte[] data, int offset, int count) {
            if (this.IsSending)
                return false; // 说明当前有没发完的数据正在发送, 该次send无效, 需要下次再尝试send

            this.SendAsync_Internal(data, offset, count);
            return true;
        }

        public void Disconnect() {
            this.Dispose();
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();
            this.RecvBufferGetter = null;
            this.SendBufferGetter = null;
            this.OnRecvData = null;
            this.OnSendData = null;
        }
    }
}