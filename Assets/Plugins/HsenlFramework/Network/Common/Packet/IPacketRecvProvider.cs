using System;

namespace Hsenl.Network {
    // 提供接收缓冲区
    public interface IPacketRecvProvider : IPacketHandler {
        public byte[] GetRecvBuffer(int len, out int offset, out int count);
        public Memory<byte> GetRecvBuffer(int length);
    }
}