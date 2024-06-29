using System;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 使用与TcpSocket相同, 区别在于数据收发时, 额外提供endpoint信息
    public class UdpSocket : IOCPSocket {
        public Func<int, (byte[] buffer, int offset, int count)> RecvBufferGetter { get; set; }
        public Func<int, (byte[] buffer, int offset, int count, EndPoint remoteEndPoint)> SendBufferGetter { get; set; }

        public Action<EndPoint, Memory<byte>> OnRecvFromData { get; set; }
        public Action<EndPoint, Memory<byte>> OnSendToData { get; set; }

        public UdpSocket(AddressFamily addressFamily) : base(new Socket(addressFamily, SocketType.Dgram, ProtocolType.Udp), IOCPSocketBufferKind.Array) { }

        public UdpSocket(Socket socket) : base(socket, IOCPSocketBufferKind.Array) {
            if (socket.ProtocolType != ProtocolType.Udp)
                throw new ArgumentException();

            if (socket.SocketType != SocketType.Dgram)
                throw new ArgumentException();
        }

        protected override Memory<byte> GetRecvBuffer(int length) {
            throw new NotImplementedException();
        }

        protected override Memory<byte> GetSendBuffer(int length) {
            throw new NotImplementedException();
        }

        protected override Memory<byte> GetSendToBuffer(int length, out EndPoint remoteEndPoint) {
            throw new NotImplementedException();
        }

        protected override byte[] GetRecvBuffer(int length, out int offset, out int count) {
            var tuple = this.RecvBufferGetter(length);
            offset = tuple.offset;
            count = tuple.count;
            return tuple.buffer;
        }

        protected override byte[] GetSendBuffer(int length, out int offset, out int count) {
            throw new NotImplementedException();
        }

        protected override byte[] GetSendToBuffer(int length, out int offset, out int count, out EndPoint remoteEndPoint) {
            // 不同于使用RecvFrom, 使用SendTo的时候, 必须提供一个有效的remoteEP, 所以在获取的发送缓存的时候, 需要明确的告诉外部, 这次SendTo是向哪发的, 以避免获取的数据并不是我们想
            // 要发送的remoteEP.
            var tuple = this.SendBufferGetter(length);
            offset = tuple.offset;
            count = tuple.count;
            remoteEndPoint = tuple.remoteEndPoint;
            return tuple.buffer;
        }

        protected override void OnRecvFrom(EndPoint remoteEndPoint, byte[] data, int offset, int count) {
            try {
                this.OnRecvFromData?.Invoke(remoteEndPoint, data.AsMemory(offset, count));
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnSendTo(EndPoint remoteEndPoint, byte[] data, int offset, int count) {
            try {
                this.OnSendToData?.Invoke(remoteEndPoint, data.AsMemory(offset, count));
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void StartReceiveFromAsync() {
            this.StartRecvFromAsync_Internal();
        }

        public void StartSendToAsync() {
            if (this.IsSending)
                return; // 已经在发了, 不需要再开始了

            this.StartSendToAsync_Internal();
        }

        public bool SendToAsync(byte[] data, int offset, int count, EndPoint remoteEndPoint) {
            if (this.IsSending)
                return false; // 说明当前有没发完的数据正在发送, 该次send无效, 需要下次再尝试send

            this.SendToAsync_Internal(data, offset, count, remoteEndPoint);
            return true;
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();
            this.RecvBufferGetter = null;
            this.SendBufferGetter = null;
            this.OnRecvFromData = null;
            this.OnSendToData = null;
        }
    }
}