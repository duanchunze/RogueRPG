using System;
using System.Collections.Generic;
using System.Net;

namespace Hsenl.Network {
    public sealed class TcpClient : Connector {
        private TcpConnector _connector;
        private TcpChannel _channel;

        public Configure Config { get; private set; }
        public Container Container { get; private set; }
        public bool IsConnected => this._channel != null;

        private readonly object _locker = new();

        public TcpClient(Configure config) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));

            this.Container = new Container();
            this.Container.Let<TcpChannel>().AllowAutoInjection();
            this.Container.Let<TcpPacketReceiver>().As<IPacketRecvProvider>().ShareInjection();
            this.Container.Let<TcpPacketSender>().As<IPacketSendProvider>().ShareInjection();
            this.Container.Let<TcpPacketReceiver>().As<IMessageReader>().ShareInjection();
            this.Container.Let<TcpPacketSender>().As<IMessageWriter>().ShareInjection();
        }

        public TcpClient(Configure config, Container container) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        /// <returns>0: 成功, 1: 正在连接, 2: 连接失败, -1: 已经连接了</returns>
        public override async HTask<int> ConnectAsync() {
            this.CheckDisposedException();
            var remoteEndPoint = this.Config.GetRemoteIPEndPoint();
            if (remoteEndPoint == null)
                throw new Exception("Remote IPEndPoint Invalid!");

            this._connector ??= new TcpConnector(remoteEndPoint.AddressFamily) {
                RecvBufferSize = this.Config.RecvBufferSize,
                SendBufferSize = this.Config.SendBufferSize
            };

            var ret = await this._connector.ConnectAsync(remoteEndPoint);
            if (ret != 0)
                return ret;

            this._channel = this.CreateChannel();
            if (this._channel == null)
                throw new Exception("Create Channel Fail!");

            this._channel.UserToken = this._connector;
            this._channel.OnRecvMessage = this.OnChannelRecvMessage;
            this._channel.OnSendMessage = this.OnChannelSendMessage;
            this._channel.OnError = this.OnChannelError;
            var channelId = SnowflakeIdGenerator.GenerateId();
            this._channel.Init(channelId);

            this._connector.RecvBufferGetter = this._channel.GetRecvBuffer;
            this._connector.SendBufferGetter = this._channel.GetSendBuffer;
            this._connector.OnRecvData = data => { this.OnChannelRecvData(this._channel.ChannelId, data); };
            this._connector.OnSendData = data => { this.OnChannelSendData(this._channel.ChannelId, data); };
            this._connector.StartReceiveAsync();

            Log.Info($"连接到了服务器: {this._connector.Socket.RemoteEndPoint}");

            foreach (var plug in this.ForeachPlugs<IOnChannelStarted>()) {
                try {
                    plug.Handle(this._channel.ChannelId);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            return ret;
        }

        private TcpChannel CreateChannel() {
            this.Container.StartStage();
            var channel = this.Container.Resolve<TcpChannel>();
            this.Container.EndStage();
            return channel;
        }

        protected override void OnChannelRecvData(long channelId, Memory<byte> data) {
            this._channel?.Read(data);
            base.OnChannelRecvData(channelId, data);
        }

        protected override void OnChannelError(long channelId, int errorCode) {
            base.OnChannelError(channelId, errorCode);

            if (errorCode >= ErrorCode.SocketError)
                return;

            Log.Error($"TcpClient Error: {errorCode}");

            this.Disconnect();
        }

        public override void Write(long channelId, byte[] data, int offset, int count) {
            if (!this.IsConnected) {
                Log.Error("Client is disconnected");
                return;
            }

            this._channel.Write(data, offset, count);
        }

        public override void Write(long channelId, Span<byte> data) {
            if (!this.IsConnected) {
                Log.Error("Client is disconnected");
                return;
            }

            this._channel.Write(data);
        }

        public override void StartSend(long channelId) {
            if (!this.IsConnected) {
                Log.Error("Client is disconnected");
                return;
            }

            this._connector.StartSendAsync();
        }

        public bool Send(IList<ArraySegment<byte>> buffers) {
            if (this._connector.IsSending)
                return false;

            return this._connector.SendPacketsAsync(buffers);
        }

        public override void Disconnect() {
            // 当channel发生错误导致被disconnect的时候, 是通过多线程调用的, 所以加锁
            lock (this._locker) {
                if (this._channel != null) {
                    foreach (var plug in this.ForeachPlugs<IOnChannelDisconnect>()) {
                        try {
                            plug.Handle(this._channel.ChannelId);
                        }
                        catch (Exception e) {
                            Log.Error(e);
                        }
                    }

                    this._channel.Dispose();
                    this._channel = null;
                }
            }
        }

        public override Channel GetChannel(long channelId) {
            return this._channel;
        }

        public override void Dispose() {
            lock (this._locker) {
                if (this.IsDisposed)
                    return;

                base.Dispose();

                this.Disconnect();

                this.Config.Dispose();
                this.Config = null;
                this._connector.Dispose();
                this._connector = null;
                this.Container.Dispose();
            }
        }

        public class Configure {
            public string RemoteIPHost { get; set; }

            public int Port { get; set; }

            public int RecvBufferSize { get; set; }

            public int SendBufferSize { get; set; }

            public IPEndPoint GetRemoteIPEndPoint() {
                if (IPAddress.TryParse(this.RemoteIPHost, out var address)) {
                    var endPoint = new IPEndPoint(address, this.Port);
                    return endPoint;
                }

                return null;
            }

            public void Reset() {
                this.RemoteIPHost = null;
                this.Port = 0;
                this.RecvBufferSize = 0;
                this.SendBufferSize = 0;
            }

            public void Dispose() {
                this.Reset();
            }
        }
    }
}