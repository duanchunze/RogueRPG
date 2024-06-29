using System;

namespace Hsenl.Network {
    // 提供发送缓冲区
    public interface IPacketSendProvider : IPacketHandler {
        public byte[] GetSendBuffer(int len, out int offset, out int count);

        // 获取发送缓冲区, 如果返回的memory长度为0, 代表当前没有需要发送的
        public Memory<byte> GetSendBuffer(int length);
    }
}