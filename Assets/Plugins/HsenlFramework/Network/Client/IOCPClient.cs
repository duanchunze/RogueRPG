using System;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    public class IOCPClient {
        private Socket _connecter;
        private SocketAsyncEventArgs _connecterEventArgs;
        private SocketAsyncEventArgs _recvEventArgs;
        private SocketAsyncEventArgs _sendEventArgs;

        public IOCPClient() {
            this._connecterEventArgs = new();
            this._recvEventArgs = new();
            this._sendEventArgs = new();
            this._recvEventArgs.SetBuffer(new byte[1024], 0, 1024);
            this._sendEventArgs.SetBuffer(new byte[1024], 0, 1024);
            this._connecterEventArgs.Completed += this.ConnecterEventArg_OnCompleted;
            this._recvEventArgs.Completed += this.RecvEventArgs_OnCompleted;
            this._sendEventArgs.Completed += this.SendEventArgs_OnCompleted;
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
            this._recvEventArgs.UserToken = e.ConnectSocket;
            this._sendEventArgs.UserToken = e.ConnectSocket;

            this.ReceiveAsync(this._recvEventArgs);
        }

        // 接收数据
        private void ReceiveAsync(SocketAsyncEventArgs e) {
            var socket = (Socket)e.UserToken;
            if (!socket.ReceiveAsync(e)) {
                this.ProcessRecv(e);
            }
        }

        private void RecvEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Receive)
                    return;

                this.ProcessRecv(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        private void ProcessRecv(SocketAsyncEventArgs e) {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success) {
                var str = e.Buffer.ToStr(e.Offset, e.BytesTransferred);
                Log.Info($"接收到来自服务器的消息: {str}");
                this.ReceiveAsync(e);
            }
        }

        // 发送数据
        private void SendAsync(SocketAsyncEventArgs e) {
            var socket = (Socket)e.UserToken;
            if (!socket.SendAsync(e)) {
                this.ProcessSend(e);
            }
        }

        private void SendEventArgs_OnCompleted(object sender, SocketAsyncEventArgs e) {
            try {
                if (e.LastOperation != SocketAsyncOperation.Send)
                    return;

                this.ProcessSend(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e) {
            if (e.SocketError == SocketError.Success) { }
        }

        // 发送消息
        public void Send(byte[] buffer, int offset, int len) {
            this._sendEventArgs.SetBuffer(buffer, offset, len);
            this.SendAsync(this._sendEventArgs);
        }

        public void Close() {
            if (this._connecter == null)
                return;

            this._connecterEventArgs.Completed -= this.ConnecterEventArg_OnCompleted;
            this._recvEventArgs.Completed -= this.RecvEventArgs_OnCompleted;
            this._sendEventArgs.Completed -= this.SendEventArgs_OnCompleted;
            this._connecterEventArgs = null;
            this._recvEventArgs = null;
            this._sendEventArgs = null;
            this._connecter.Close();
            this._connecter = null;
        }
    }
}