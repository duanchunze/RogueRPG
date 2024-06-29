using System;
using System.Buffers;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    public class KcpChannel : Channel {
        private IPacketHandler _packetHandler;
        private IKcpHandler _kcpHandler;
        private IPacketRecvProvider _recvProvider;
        private IPacketSendProvider _sendProvider;
        private IMessageReader _packetReader;
        private IMessageWriter _packetWriter;

        public EndPoint RemoteEndPoint { get; private set; }

        internal void Init(long channelId, EndPoint remoteEndPoint, ArrayPool<byte> arrayPool) {
            base.Init(channelId);
            this._packetReader.OnMessageReaded = message => { this.OnRecvMessage?.Invoke(this.ChannelId, message); };
            this._packetWriter.OnMessageWrited = message => { this.OnSendMessage?.Invoke(this.ChannelId, message); };

            this.RemoteEndPoint = remoteEndPoint;
            this._packetHandler.Init(this);
            this._kcpHandler.Init((uint)channelId, arrayPool);
        }

        internal uint Update(uint currentTimeMS) {
            return this._kcpHandler.Update(currentTimeMS);
        }

        internal override Memory<byte> GetRecvBuffer(int len) {
            return this._recvProvider.GetRecvBuffer(len);
        }

        internal override Memory<byte> GetSendBuffer(int len) {
            return this._sendProvider.GetSendBuffer(len);
        }

        internal override byte[] GetRecvBuffer(int len, out int offset, out int count) {
            return this._recvProvider.GetRecvBuffer(len, out offset, out count);
        }

        internal override byte[] GetSendBuffer(int len, out int offset, out int count) {
            return this._sendProvider.GetSendBuffer(len, out offset, out count);
        }

        internal override void Read(Memory<byte> data) {
            try {
                this._packetReader.Read(data);
            }
            catch {
                this.Error(ErrorCode.Error_MessageReadFailure);
                throw;
            }
        }

        internal override void Write(byte[] data, int offset, int count) {
            try {
                this._packetWriter.Write(data, offset, count);
            }
            catch {
                this.Error(ErrorCode.Error_MessageWriteFailure);
                throw;
            }
        }

        internal override void Write(Span<byte> data) {
            try {
                this._packetWriter.Write(data);
            }
            catch {
                this.Error(ErrorCode.Error_MessageWriteFailure);
                throw;
            }
        }

        internal override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();

            this._packetHandler.Dispose();
            this._recvProvider.Dispose();
            this._sendProvider.Dispose();
            this._packetReader.Dispose();
            this._packetWriter.Dispose();
        }
    }
}