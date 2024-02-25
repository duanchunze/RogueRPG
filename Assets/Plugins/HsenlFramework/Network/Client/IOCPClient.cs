using System;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    public class IOCPClient : IOCPChannel {
        private Socket _connecter;
        private SocketAsyncEventArgs _connecterEventArgs;

        /// <param name="recvBufferCapacity">每个接收缓冲区的大小</param>
        /// <param name="maxinumSendSizeOnce">每次发送的最大数据</param>
        public IOCPClient(int recvBufferCapacity, int maxinumSendSizeOnce) {
            this._connecterEventArgs = new();
            this._connecterEventArgs.Completed += this.ConnecterEventArg_OnCompleted;
            this.Init(recvBufferCapacity, maxinumSendSizeOnce);
        }

        // 开始连接
        public void StartConnecting(IPEndPoint remoteEndPoint) {
            if (this._connecter != null)
                throw new Exception("Client is already started!");

            this._connecter = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._connecterEventArgs.RemoteEndPoint = remoteEndPoint;
            if (!this._connecter.ConnectAsync(this._connecterEventArgs)) {
                this.ProcessConnect(this._connecterEventArgs);
            }
        }

        private void ConnecterEventArg_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                this.ProcessConnect(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Connect)
                return;

            Log.Info($"连接到了服务器: {e.ConnectSocket.AddressFamily}");
            // 启动, 开始接收服务器数据
            this.Start(e.ConnectSocket);
        }

        public override void Close() {
            if (this._connecter == null)
                return;

            base.Close();

            this._connecterEventArgs.Completed -= this.ConnecterEventArg_OnCompleted;
            this._connecterEventArgs = null;
            this._connecter.Close();
            this._connecter = null;
        }
    }
}