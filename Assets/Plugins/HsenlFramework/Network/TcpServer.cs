using System;

namespace Hsenl.Network {
    public class TcpServer : Component, IUpdate {
        private IServer _server;

        public void Init<T>() where T : IServer, new() {
            this._server = new T();
        }

        public void Update() {
            
        }
    }
}