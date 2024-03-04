using System;

namespace Hsenl.Network {
    public class Reconnect : IPlug, IOnChannelDisconnected {
        private IPluggable _pluggable;
        private int _reconnectInterval;
        private int _tryNum;

        public Reconnect(int reconnectIntervalSeconds = 5, int tryNum = 5) {
            this._reconnectInterval = reconnectIntervalSeconds;
            this._tryNum = tryNum;
        }

        public void Init(IPluggable pluggable) {
            if (pluggable is not IClient)
                Log.Error($"KeepalivePlug only using on client! '{pluggable.GetType()}'");

            this._pluggable = pluggable;
        }

        public void Dispose() {
            this._pluggable = null;
            this._reconnectInterval = 0;
            this._tryNum = 0;
        }

        public void Handle(long channelId) {
            try {
                this.Start(channelId);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void Start(long channelId) {
            this.ContinueReconnect().Tail();
        }

        private async HVoid ContinueReconnect() {
            var tryCount = this._tryNum;
            while (tryCount-- > 0) {
                if (((IClient)this._pluggable).IsClosed)
                    return;

                ((IClient)this._pluggable).Start();
                await Timer.WaitTime(this._reconnectInterval * 1000);
                if (this._pluggable == null)
                    return;
                if (((IClient)this._pluggable).IsConnecting)
                    break;
            }
        }
    }
}