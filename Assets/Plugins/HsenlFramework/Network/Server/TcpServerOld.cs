// using System;
// using System.Collections.Generic;
// using System.Diagnostics.CodeAnalysis;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
//
// namespace Hsenl.Network.Server {
//     public class TcpServerOld {
//         private readonly int _port;
//         private TcpListener _tcpListener;
//
//         private Task _listeningTask;
//         private List<TcpClient> _clients = new();
//
//         public TcpServerOld(int port) {
//             this._port = port;
//         }
//
//         public async void Listening() {
//             Log.Info("线程ID: " + Thread.CurrentThread.ManagedThreadId);
//             this._tcpListener = new TcpListener(IPAddress.Any, this._port);
//             this._tcpListener.Start();
//
//             Log.Info($"服务器已启动，监听在：{IPAddress.Any}:{this._port}");
//
//             while (true) {
//                 TcpClient client = null;
//                 try {
//                     client = await this._tcpListener.AcceptTcpClientAsync()!;
//                     Log.Info("线程ID: " + Thread.CurrentThread.ManagedThreadId);
//                 }
//                 catch (Exception e) {
//                     Log.Error(e);
//                     return;
//                 }
//
//                 if (client != null) {
//                     Log.Info($"接收到客户端连接: {client}");
//                     this._clients.Add(client);
//                     HandleClient(client).Coroutine();
//                 }
//             }
//             // ReSharper disable once FunctionNeverReturns
//         }
//
//         public bool IsClosed {
//             get {
//                 if (this._tcpListener == null)
//                     return true;
//                 try {
//                     return !this._tcpListener.Server.IsBound;
//                 }
//                 catch (Exception e) {
//                     return true;
//                 }
//             }
//         }
//
//         private static async ETTask HandleClient(TcpClient client) {
//             try {
//                 using var stream = client.GetStream();
//                 byte[] buffer = new byte[1024];
//                 while (true) {
//                     // 读取客户端发送的数据
//                     Log.Info("等待客户端消息: ");
//                     int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
//                     Log.Info("读取到客户端消息: ");
//                     if (bytesRead == 0) {
//                         Log.Info("等一帧");
//                         await Timer.WaitFrame();
//                         Log.Error("Continue");
//                         continue;
//                         break;
//                     }
//
//                     var message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
//                     Log.Info("从客户端收到消息: " + message);
//
//                     byte[] responseBuffer = Encoding.UTF8.GetBytes($"服务端回复：我听到你说'{message}'了");
//                     await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
//                 }
//             }
//             catch (Exception e) {
//                 Log.Error($"处理客户端时发生错误: {e.Message}");
//                 client.Close();
//             }
//         }
//
//         public async ETTask SendMessage(string message) {
//             foreach (var client in this._clients) {
//                 using var stream = client.GetStream();
//                 byte[] responseBuffer = Encoding.UTF8.GetBytes($"服务端对你说'{message}'了");
//                 await stream.WriteAsync(responseBuffer, 0, responseBuffer.Length);
//                 // try {
//                 //     
//                 // }
//                 // catch (Exception e) {
//                 //     Log.Error($"处理客户端时发生错误: {e.Message}");
//                 // }
//                 // finally {
//                 //     client.Close();
//                 // }
//             }
//         }
//
//         public void Stop() {
//             var temp = this._tcpListener;
//             this._tcpListener = null;
//             temp?.Stop();
//             Log.Info("服务器已停止");
//         }
//     }
// }