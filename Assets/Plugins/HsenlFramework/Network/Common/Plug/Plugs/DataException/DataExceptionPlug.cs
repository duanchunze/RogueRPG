namespace Hsenl.Network {
    // 数据异常, 比如消息总是无法正确解析, 或者其他异常
    public class DataExceptionPlug : IPlug, IOnMessageReadFailure {
        public void Init(IPluggable pluggable) {
            throw new System.NotImplementedException();
        }

        public void Dispose() {
            throw new System.NotImplementedException();
        }

        // 如果总是错误, 则断开用户
        public void Handle(long channelId, int errorCode) {
            throw new System.NotImplementedException();
        }
    }
}