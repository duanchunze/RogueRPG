using System;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 使用: 调用StartAccept(), 然后等待OnAccepted的通知
    // 因为socket被bind ip作为server之后, 就不承担数据收发作用了, 所以单独把acceptor独立出来
    public class TcpAcceptor : EpibolySocket {
        private SocketAsyncEventArgs _acceptEventArgs;

        public Action<Socket> OnAccepted { get; set; }

        public TcpAcceptor(EndPoint localEndPoint) : base(new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)) {
            this.Socket.Bind(localEndPoint);
            this._acceptEventArgs = new SocketAsyncEventArgs();
            this._acceptEventArgs.Completed += this.AcceptEventArg_Completed;
        }

        public TcpAcceptor(Socket socket) : base(socket) {
            if (socket.ProtocolType != ProtocolType.Tcp)
                throw new ArgumentException();

            if (socket.SocketType != SocketType.Stream)
                throw new ArgumentException();

            this._acceptEventArgs = new SocketAsyncEventArgs();
            this._acceptEventArgs.Completed += this.AcceptEventArg_Completed;
        }

        public void StartAccept() {
            this.AcceptAsync(this._acceptEventArgs);
        }

        // 接受客户端连接
        private void AcceptAsync(SocketAsyncEventArgs e) {
            e.AcceptSocket = null;
            if (!this.Socket.AcceptAsync(e)) {
                this.ProcessAccept(e);
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e) {
            if (this.Socket == null)
                return;

            try {
                // 处理这个连接请求
                this.ProcessAccept(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }

            // 继续接受其他客户端连接
            this.AcceptAsync(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Accept)
                return;

            try {
                this.OnAccepted?.Invoke(e.AcceptSocket);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();

            this._acceptEventArgs.Completed -= this.AcceptEventArg_Completed;
            this._acceptEventArgs = null;

            this.OnAccepted = null;
        }
    }
}