using System;

namespace Hsenl.Network {
    public interface IAfterMessageReaded : IPlugGroup {
        public void Handle(ref Memory<byte> data);
    }
}