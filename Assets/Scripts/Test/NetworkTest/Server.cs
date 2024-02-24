using System.Net;
using Hsenl;
using Hsenl.Network;

namespace Hsenl {
    public class Server : Component, IUpdate {
        public static Server Instance;

        public IOCPServer IocpServer { get; private set; }

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            this.IocpServer = new IOCPServer(3, 1024);
            IPAddress.TryParse("127.0.0.1", out var address);
            var endPoint = new IPEndPoint(address, 12312);
            this.IocpServer.Init();
            this.IocpServer.StartListening(endPoint);
        }

        public void Update() {
            
        }
    }
}