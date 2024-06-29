namespace Hsenl.Network {
    public interface IOnMessageReadFailure : IPlugGroup {
        public void Handle(long channelId, int errorCode);
    }
}