// using System;
// using System.Collections;
// using System.Text;
//
// namespace Hsenl.Network.Client {
//     public class TcpClient {
//         private readonly string _serverIpAddress;
//         private readonly int _serverPort;
//         private System.Net.Sockets.TcpClient _tcpConnector;
//
//         public TcpClient(string serverIp, int serverPort) {
//             this._serverIpAddress = serverIp;
//             this._serverPort = serverPort;
//             this._tcpConnector = new System.Net.Sockets.TcpClient();
//         }
//
//         public async ETTask ConnectToServer() {
//             Log.Info($"客户端尝试连接服务端: {this._serverIpAddress}:{this._serverPort}");
//             try {
//                 await this._tcpConnector.ConnectAsync(this._serverIpAddress, this._serverPort);
//
//                 using var stream = this._tcpConnector.GetStream();
//
//                 // 发送消息
//                 string messageToSend = "Hello, Server!";
//                 byte[] buffer = Encoding.UTF8.GetBytes(messageToSend);
//                 await stream.WriteAsync(buffer, 0, buffer.Length);
//
//                 // 接收消息
//                 byte[] receiveBuffer = new byte[1024];
//                 int bytesRead = await stream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);
//                 string receivedMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
//
//                 Log.Info("Received from server: " + receivedMessage);
//             }
//             catch (Exception e) {
//                 Log.Info("Error while connecting or communicating: " + e.Message);
//             }
//         }
//
//         public IEnumerator CheckConnect() {
//             Log.Info("持续检查客户端是否处于连接中...");
//             while (true) {
//                 if (this._tcpConnector == null)
//                     break;
//
//                 if (!this._tcpConnector.Connected) {
//                     yield return null;
//                     continue;
//                 }
//
//                 Log.Info("客户端处于连接中...");
//                 yield return null;
//             }
//             
//             Log.Info("客户端已经断开了");
//         }
//
//         public void Stop() {
//             this._tcpConnector.Close();
//         }
//
//         public bool IsConnect() {
//             return this._tcpConnector?.Connected ?? false;
//         }
//     }
// }