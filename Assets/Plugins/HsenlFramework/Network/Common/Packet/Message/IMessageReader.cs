using System;

namespace Hsenl.Network {
    // 从缓存区中, 读取出可用的消息数据
    public interface IMessageReader : IPacketHandler {
        public Action<Memory<byte>> OnMessageReaded { get; set; }
        public void Read(Memory<byte> data);
    }
}