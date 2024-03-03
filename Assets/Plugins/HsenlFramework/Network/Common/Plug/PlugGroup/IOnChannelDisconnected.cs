namespace Hsenl.Network {
    public interface IOnChannelDisconnected : IPlugGroup {
        public void Handle(long channelId);
    }
}