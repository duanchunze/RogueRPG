using System;

namespace Hsenl.Network {
    public interface IAfterMessageWrited : IPlugGroup {
        public void Handle(long channel, ref Memory<byte> data);
    }
}