namespace Hsenl.Network {
    public interface IOnChannelStarted : IPlugGroup {
        public void Handle(long channelId);
    }
}