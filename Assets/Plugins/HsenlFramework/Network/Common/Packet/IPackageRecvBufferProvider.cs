using System;

namespace Hsenl.Network {
    // 提供接收缓冲区
    public interface IPackageRecvBufferProvider : IPackageHandler {
        public Memory<byte> GetRecvBuffer(int len);
    }
}