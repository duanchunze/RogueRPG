using System;
using System.Net.Sockets;

namespace Hsenl.Network {
    public abstract class Channel : IChannel {
        private int _recvBufferSize;
        private int _sendBufferSize;

        private long _channelId;
        private Socket _socket;

        protected int totalBytesRecv; // 记录总共接收字节数
        protected int totalBytesSend; // 记录总共发送字节数

        private bool _isSending;
        private bool _isDisposed;

        public Action<long, Memory<byte>> OnRecvPackage { get; set; }
        public Action<long, ushort, Memory<byte>> OnRecvMessage { get; set; }
        public Action<long, Memory<byte>> OnSendPackage { get; set; }
        public Action<long, ushort, Memory<byte>> OnSendMessage { get; set; }
        public Action<long, int> OnError { get; set; }

        public int RecvBufferSize => this._recvBufferSize;

        public int SendBufferSize => this._sendBufferSize;

        public long ChannelId {
            get => this._channelId;
            set => this._channelId = value;
        }

        public Socket Socket {
            get => this._socket;
            protected set => this._socket = value;
        }

        public bool IsSending {
            get => this._isSending;
            protected set => this._isSending = value;
        }

        public bool IsDisposed {
            get => this._isDisposed;
            protected set => this._isDisposed = value;
        }

        public int TotalBytesRecv => this.totalBytesRecv;

        public int TotalBytesSend => this.totalBytesSend;

        public void Init(int recvBufferSize, int sendBufferSize) {
            this._recvBufferSize = recvBufferSize;
            this._sendBufferSize = sendBufferSize;
        }

        // 启动
        public abstract void Start(Socket s);

        public abstract void Write(Func<PackageBuffer, ushort> func);

        public abstract bool Send();

        // 断开连接, 并把所有数据重置
        public virtual void Disconnect() {
            if (this.IsDisposed)
                throw new Exception("Channel is closed!");

            this._recvBufferSize = 0;
            this._sendBufferSize = 0;
            this._channelId = 0;
            if (this._socket != null) {
                this._socket.Close();
                this._socket = null;
            }

            this.totalBytesRecv = 0;
            this.totalBytesSend = 0;
            this.IsSending = false;
            this.OnRecvPackage = null;
            this.OnRecvMessage = null;
            this.OnSendPackage = null;
            this.OnSendMessage = null;
            this.OnError = null;
        }

        public virtual void Dispose() {
            if (this.IsDisposed)
                return;

            this.IsDisposed = true;
        }
    }
}