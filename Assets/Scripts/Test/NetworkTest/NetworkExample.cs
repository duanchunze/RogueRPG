using Hsenl;
using Hsenl.Network;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Test {
    public class NetworkExample : MonoBehaviour {
        private NetworkServer _server;
        private NetworkClient _client;

        [Button("Click")]
        public void Click() { }

        private void Start() {
            SceneManager.LoadScene("main", LoadSceneMode.Single);

            this._server = Entity.Create("Server").AddComponent<NetworkServer>();
            this._client = Entity.Create("Client").AddComponent<NetworkClient>();

            {
                // server
                var kcpServer = new KcpServer(new KcpServer.Configure() {
                    LocalIPHost = "127.0.0.1",
                    Port = 12312,
                    RecvBufferSize = 512,
                    SendBufferSize = 512,
                    Backlog = 1000,
                });

                var encryptionPlug = new DefaultEncryptionPlug();
                kcpServer.AddPlug<IAfterMessageReaded, EncryptionPlug>(encryptionPlug);
                kcpServer.AddPlug<IAfterMessageWrited, EncryptionPlug>(encryptionPlug);
                TrafficMonitoringPlug trafficMonitoringPlug = new();
                kcpServer.AddPlug<IOnChannelStarted, TrafficMonitoringPlug>(trafficMonitoringPlug);
                kcpServer.AddPlug<IOnChannelDisconnect, TrafficMonitoringPlug>(trafficMonitoringPlug);
                kcpServer.AddPlug<IOnChannelStarted, KeepalivePlug>(new KeepalivePlug());
                this._server.Start(kcpServer);
            }

            {
                // client
                var kcpClient = new KcpClient(new KcpClient.Configure() {
                    RemoteIPHost = "127.0.0.1",
                    Port = 12312,
                    RecvBufferSize = 512,
                    SendBufferSize = 512,
                });

                var encryptionPlug = new DefaultEncryptionPlug();
                kcpClient.AddPlug<IAfterMessageReaded, EncryptionPlug>(encryptionPlug);
                kcpClient.AddPlug<IAfterMessageWrited, EncryptionPlug>(encryptionPlug);
                kcpClient.AddPlug<IOnChannelDisconnect, ReconnectPlug>(new ReconnectPlug());
                this._client.Start(kcpClient);
            }
        }

        private async void Login() {
            var now = TimeInfo.Now;
            var response = await this._client.Call(new C2R_Login() {
                account = "duanchunzeduanchunze",
                password = "aga;jag;dfa;lg;a"
            }).As<R2C_Login>();
            Debug.LogWarning(response.ip + $"耗时: {TimeInfo.Now - now}");
        }

        private async void Update() {
            if (InputController.GetButtonDown(InputCode.L)) {
                this.Login();
            }

            if (InputController.GetButton(InputCode.K)) {
                this.Login();
            }
        }
    }
}