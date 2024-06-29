using System;

namespace Hsenl.Network {
    public interface IOnSendData : IPlugGroup {
        public void Handle(long channel, ref Memory<byte> data);
    }
}