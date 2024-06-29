using System;

namespace Hsenl.Network {
    public abstract class Connector : Service {
        public abstract HTask<int> ConnectAsync();
        public abstract void Disconnect();
    }
}