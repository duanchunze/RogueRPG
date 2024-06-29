using System;

namespace Hsenl.Network {
    public abstract class Acceptor : Service {
        public abstract void StartAccept();
        public abstract void DisconnectChannel(long channelId);
    }
}