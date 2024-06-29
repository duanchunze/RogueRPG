using System;
using System.Net;
using System.Net.Sockets;

namespace Hsenl.Network {
    // 使用: 提供一键直连的方法
    // connector不同于acceptor, 后者作为接受者之后, 便不能再用于数据收发, 而前者则依然可以用于数据收发, 所以connector继承的是TcpSocket
    public class TcpConnector : TcpSocket {
        public bool IsConnecting { get; private set; }
        public bool IsConnected { get; private set; }

        public TcpConnector(AddressFamily addressFamily) : base(addressFamily) { }

        public TcpConnector(Socket socket) : base(socket) { }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="remoteEndPoint"></param>
        /// <returns>0: 成功, 1: 正在连接, 2: 连接失败, -1: 已经连接了</returns>
        public async HTask<int> ConnectAsync(IPEndPoint remoteEndPoint) {
            this.CheckDisposedException();

            if (this.IsConnected)
                return -1;

            if (this.IsConnecting)
                return 1;

            this.IsConnecting = true;
            try {
                await this.Socket.ConnectAsync(remoteEndPoint);
                await HTask.ReturnToMainThread();
            }
            catch (SocketException e) {
                this.Error(e.ErrorCode);
                return 2;
            }
            catch (Exception e) {
                Log.Error(e);
                return 2;
            }
            finally {
                this.IsConnecting = false;
            }

            this.IsConnected = true;
            return 0;
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();
            this.IsConnected = false;
        }
    }
}