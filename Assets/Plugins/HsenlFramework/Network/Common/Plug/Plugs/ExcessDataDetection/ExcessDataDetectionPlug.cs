using System;

namespace Hsenl.Network {
    // 检测用户发送给服务器的数据是否异常且持续的大
    public class ExcessDataDetectionPlug : IPlug, IAfterMessageReaded {
        public void Init(IPluggable pluggable) {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }

        public void Handle(long channel, ref Memory<byte> data) {
            throw new NotImplementedException();
        }
    }
}