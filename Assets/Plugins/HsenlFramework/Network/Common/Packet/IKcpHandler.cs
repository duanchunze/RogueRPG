using System.Buffers;

namespace Hsenl.Network {
    public interface IKcpHandler {
        public void Init(uint conv, ArrayPool<byte> arrayPool);
        public uint Update(uint currentTimeMS);
    }
}