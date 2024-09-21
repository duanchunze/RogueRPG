// using System.Threading;
// using Hsenl;
// using Hsenl.Network;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace Test {
//     public class NetworkExample : MonoBehaviour {
//         private NetworkServer _server;
//         private NetworkClient _client;
//
//         [Button("Click")]
//         public void Click() { }
//
//         private void Start() {
//             SceneManager.LoadScene("main", LoadSceneMode.Single);
//
//             
//             {
//                 // server
//                 // var server = new KcpServer(new KcpServer.Configure() { // 使用kcp
//                 var server = new TcpServer(new TcpServer.Configure() { // 使用tcp
//                     LocalIPHost = "127.0.0.1",
//                     Port = 12312,
//                     RecvBufferSize = 512,
//                     SendBufferSize = 512,
//                     Backlog = 1000,
//                 });
//
//                 var encryptionPlug = new DefaultEncryptionPlug();
//                 server.AddPlug<IAfterMessageReaded, EncryptionPlug>(encryptionPlug);
//                 server.AddPlug<IAfterMessageWrited, EncryptionPlug>(encryptionPlug);
//                 TrafficMonitoringPlug trafficMonitoringPlug = new();
//                 server.AddPlug<IOnChannelStarted, TrafficMonitoringPlug>(trafficMonitoringPlug);
//                 server.AddPlug<IOnChannelDisconnect, TrafficMonitoringPlug>(trafficMonitoringPlug);
//                 server.AddPlug<IAfterMessageReaded, TrafficMonitoringPlug>(trafficMonitoringPlug);
//                 server.AddPlug<IAfterMessageWrited, TrafficMonitoringPlug>(trafficMonitoringPlug);
//                 server.AddPlug<IOnChannelStarted, KeepalivePlug>(new KeepalivePlug());
//                 this._server = Entity.Create(server.GetType().Name).AddComponent<NetworkServer>();
//                 this._server.Start(server);
//             }
//
//             {
//                 // client
//                 // var client = new KcpClient(new KcpClient.Configure() { // 使用kcp
//                 var client = new TcpClient(new TcpClient.Configure() { // 使用tcp
//                     RemoteIPHost = "127.0.0.1",
//                     Port = 12312,
//                     RecvBufferSize = 512,
//                     SendBufferSize = 512,
//                 });
//
//                 var encryptionPlug = new DefaultEncryptionPlug();
//                 client.AddPlug<IAfterMessageReaded, EncryptionPlug>(encryptionPlug);
//                 client.AddPlug<IAfterMessageWrited, EncryptionPlug>(encryptionPlug);
//                 client.AddPlug<IOnChannelDisconnect, ReconnectPlug>(new ReconnectPlug());
//                 this._client = Entity.Create(client.GetType().Name).AddComponent<NetworkClient>();
//                 this._client.Start(client);
//             }
//         }
//
//         private async void Login() {
//             var now = TimeInfo.Now;
//             var response = await this._client.Call(new C2R_Login() {
//                 account = "duanchunzeduanchunze",
//                 password = "aga;jag;dfa;lg;a"
//             }).MainThreadAs<R2C_Login>();
//             Debug.LogWarning(response.ip + $"耗时: {TimeInfo.Now - now} 线程: {Thread.CurrentThread.ManagedThreadId}");
//         }
//
//         private void Update() {
//             if (InputController.GetButtonDown(InputCode.L)) {
//                 this.Login();
//             }
//
//             if (InputController.GetButton(InputCode.K)) {
//                 this.Login();
//             }
//         }
//     }
// }