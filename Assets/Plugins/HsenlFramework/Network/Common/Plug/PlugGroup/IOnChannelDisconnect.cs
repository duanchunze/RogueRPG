namespace Hsenl.Network {
    public interface IOnChannelDisconnect : IPlugGroup {
        public void Handle(long channelId);
    }
}