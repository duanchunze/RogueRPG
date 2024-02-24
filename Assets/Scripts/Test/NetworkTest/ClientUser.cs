using System.Collections.Generic;
using System.Net;
using Hsenl;
using Hsenl.Network;

namespace Hsenl {
    public class ClientUser : Component, IUpdate {
        public static ClientUser Instance;

        public IOCPClient IocpClient { get; private set; }

        private Dictionary<int, object> responses = new();

        protected override void OnAwake() {
            Instance = this;
        }

        protected override void OnStart() {
            IPAddress.TryParse("127.0.0.1", out var address);
            var endPoint = new IPEndPoint(address, 12312);
            this.IocpClient = new IOCPClient();
            this.IocpClient.StartConnecting(endPoint);
            this.IocpClient.
        }


        public void Update() {
            if (InputController.GetButtonDown(InputCode.L)) { }
        }

        private async HTask Login() {
            C2G_Login c2GLogin = new() {
                account = "123",
                password = "456"
            };

            await this.Request(new C2G_Login() { account = "123", password = "456" });
        }

        private async HTask<G2C_Login> Request<T>(T message) {
            byte[] bytes = SerializeHelper.SerializeOfMemoryPack(message);
            this.IocpClient.Send(bytes, 0, bytes.Length);
            var task = HTask<G2C_Login>.Create();
            this.responses.Add(1, task);
            return await task;
        }
    }
}