using System;

namespace Hsenl.Network {
    // 提供发送缓冲区
    public interface IPackageSendBufferProvider : IPackageHandler {
        public Memory<byte> GetSendBuffer(int min, int max);
        public bool SendNext();
    }
}