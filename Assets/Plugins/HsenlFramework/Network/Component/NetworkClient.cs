using System;

namespace Hsenl.Network {
    [Serializable]
    public sealed class NetworkClient : Network {
        private Connector _connector;
        protected override Service Service => this._connector;

        public void Start(Connector connector) {
            this._connector = connector;
            this._connector.OnRecvMessage += this.OnRecvMessage;
            this._connector.ConnectAsync().Tail();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            this._connector.Dispose();
            this._connector = null;
        }

        public void Send<T>(T message) {
            base.Send(message, 0);
        }

        public RpcInfo Call<T>(T message) {
            return base.Call(message, 0);
        }
    }
}