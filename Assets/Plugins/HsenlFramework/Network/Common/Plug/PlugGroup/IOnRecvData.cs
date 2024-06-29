using System;

namespace Hsenl.Network {
    public interface IOnRecvData : IPlugGroup {
        public void Handle(long channel, ref Memory<byte> data);
    }
}