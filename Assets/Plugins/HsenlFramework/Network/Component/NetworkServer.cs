using System;

namespace Hsenl.Network {
    [Serializable]
    public sealed class NetworkServer : Network {
        private Acceptor _acceptor;
        protected override Service Service => this._acceptor;

        public void Start(Acceptor connector) {
            this._acceptor = connector;
            this._acceptor.OnRecvMessage += this.OnRecvMessage;
            this._acceptor.StartAccept();
        }

        protected override void OnDestroy() {
            base.OnDestroy();
            this._acceptor.Dispose();
            this._acceptor = null;
        }
    }
}