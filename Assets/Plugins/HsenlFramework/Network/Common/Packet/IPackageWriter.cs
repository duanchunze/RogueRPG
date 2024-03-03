using System;

namespace Hsenl.Network {
    // 包装包, 粘包
    public interface IPackageWriter : IPackageHandler {
        public Action<ushort, Memory<byte>> OnMessageWrited { get; set; }

        // 写入后, 可以不用着急马上Send, 可以等到帧末调用一次Send, 这样既不会等待一帧的延迟, 同时也可以避免频繁的send, 占用通道.
        // 写入时机没有限制, 任意时机写都行, 但不支持多线程
        public void Write(Func<PackageBuffer, ushort> write);
    }
}