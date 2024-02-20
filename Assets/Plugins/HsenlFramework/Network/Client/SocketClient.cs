using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Hsenl.Network.Client {
    public class SocketClient {
        private Socket _connecter;

        private Thread _thread;

        public void Init() { }

        public void StartConnect(string addressStr, int port) {
            if (!IPAddress.TryParse(addressStr, out var address))
                return;

            var endPoint = new IPEndPoint(address, port);
            this.StartConnect(endPoint);
        }

        public void StartConnect(IPEndPoint endPoint) {
            this._connecter = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                this._connecter.Connect(endPoint);

                string message = "Hello, Server!";
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                this._connecter.Send(bytes);

                this._thread = new(o => {
                    int i = 30;
                    while (i-- > 0) {
                        var bs = new byte[1024];
                        int receLen = this._connecter.Receive(bs);
                        string response = Encoding.UTF8.GetString(bs, 0, receLen);

                        Log.Info($"收到服务器的消息: {response}");
                        Thread.Sleep(1);

                        this._connecter.Send(bs);
                    }
                });
                this._thread.Start();
            }
            catch (Exception e) {
                Log.Error($"连接失败: {e}");
            }
        }

        public void Close() {
            this._thread.Abort();
            this._connecter.Close();
        }
    }
}