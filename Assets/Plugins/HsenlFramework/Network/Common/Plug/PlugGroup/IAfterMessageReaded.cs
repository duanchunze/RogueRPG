using System;

namespace Hsenl.Network {
    public interface IAfterMessageReaded : IPlugGroup {
        public void Handle(long channel, ref Memory<byte> data);
    }
}