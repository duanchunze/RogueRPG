using System;

namespace Hsenl.Network {
    // 解析包, 拆包
    public interface IPackageReader : IPackageHandler {
        public Action<ushort, Memory<byte>> OnMessageReaded { get; set; }
        public void Read(Memory<byte> data);
    }
}