// See https://aka.ms/new-console-template for more information

using System.Net;
using Hsenl;
using Hsenl.Network;
using Launch;

Console.WriteLine("Hello, World!");

SingletonManager.Register<LogManager>();
LogManager.Instance.Init(new NLogger(), 1);

// var server = new IOCPServer(32, 32);
// IPAddress.TryParse("127.0.0.1", out var address);
// var endPoint = new IPEndPoint(address, 12312);
// server.StartListening(endPoint);
// server.OnRecvData += (channel, memory) => {
//     
// };

test.dfdfd();

// while (true) {
//     Thread.Sleep(17);
//
//     string? readLine = Console.ReadLine();
//     if (readLine != null) {
//         if (readLine == "start") {
//             // var client = new SocketClient();
//             // client.StartConnect(endPoint);
//         }
//
//         if (readLine == "say") {
//             
//         }
//         
//         if (readLine == "close") {
//             // server.Close();
//         }
//     }
// }
