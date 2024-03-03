using System;

namespace Hsenl.Network {
    public interface IAfterMessageWrited : IPlugGroup {
        public void Handle(ref Memory<byte> data);
    }
}