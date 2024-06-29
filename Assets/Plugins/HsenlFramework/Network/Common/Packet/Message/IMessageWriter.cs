using System;

namespace Hsenl.Network {
    // 将一个消息体写入缓存区
    public interface IMessageWriter : IPacketHandler {
        public Action<Memory<byte>> OnMessageWrited { get; set; }
        public void Write(byte[] data, int offset, int count);
        public void Write(Span<byte> data);
        
    }
}