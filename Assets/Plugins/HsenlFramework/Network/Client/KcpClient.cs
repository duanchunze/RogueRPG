using System;
using System.Buffers;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Hsenl.Network {
    public sealed class KcpClient : Connector {
        private UdpSocket _connector;
        private KcpChannel _channel;
        private EndPoint _remoteEndPoint;

        private readonly PackageBuffer _recvBuffer = new();
        private readonly byte[] _sendToCache = new byte[12];
        private HTask<int> _connectTask;
        private readonly ArrayPool<byte> _kcpArrayPool = ArrayPool<byte>.Create(2048, 100);
        private Thread _kcpThread;
        private uint _kcpUpdateTime;

        public Configure Config { get; private set; }
        public Container Container { get; private set; }
        public bool IsConnecting => !this._connectTask.IsNull();
        public bool IsConnected => this._channel != null;

        private readonly object _locker = new();

        public KcpClient(Configure config) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));

            this.Container = new Container();
            this.Container.Let<KcpChannel>().AllowAutoInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketHandler>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IKcpHandler>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketRecvProvider>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketSendProvider>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IMessageReader>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IMessageWriter>().ShareInjection();

            this.Init();
        }

        public KcpClient(Configure config, Container container) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
            this.Init();
        }

        private void Init() {
            this._remoteEndPoint = this.Config.GetRemoteIPEndPoint();
            if (this._remoteEndPoint == null)
                throw new Exception("Remote IPEndPoint Invalid!");

            this._connector = new UdpSocket(this._remoteEndPoint.AddressFamily) {
                RecvBufferSize = this.Config.RecvBufferSize,
                SendBufferSize = this.Config.SendBufferSize,
                RecvBufferGetter = len => {
                    if (len > this._recvBuffer.Length)
                        this._recvBuffer.SetLength(len);

                    var buffer = this._recvBuffer.GetBuffer();
                    return (buffer, 0, buffer.Length);
                },
                SendBufferGetter = len => {
                    if (this._channel == null)
                        return default;

                    var bytes = this._channel.GetSendBuffer(len, out int offset, out int count);
                    return (bytes, offset, count, this._channel.RemoteEndPoint);
                },
                OnRecvFromData = this.OnRecvFromData,
                OnSendToData = this.OnSendToData
            };

            this._connector.StartReceiveFromAsync();

            this._kcpThread = new Thread(this.KcpDriver);
            this._kcpThread.Start();
        }

        /// <returns>0: 成功, 1: 正在连接, 2: 连接失败, -1: 已经连接了</returns>
        public override async HTask<int> ConnectAsync() {
            this.CheckDisposedException();
            if (this.IsConnected)
                return -1;

            if (this.IsConnecting)
                return 1;

            var channelId = SnowflakeIdGenerator.GenerateId();
            this.SendSYNTo((uint)channelId, this._remoteEndPoint);

            this._connectTask = HTask<int>.Create();
            Timeout();
            var ret = await this._connectTask;
            await HTask.ReturnToMainThread();

            return ret;

            async void Timeout() {
                await Task.Delay(8000);
                this._connectTask.SetResult(2);
            }
        }

        private void OnRecvFromData(EndPoint remoteEndPoint, Memory<byte> data) {
            // 根据发来的数据, 判断是要连接, 还是发送消息
            var span = data.Span;
            var head = span[0];
            var remoteConv = BitConverter.ToUInt32(span.Slice(1));
            switch (head) {
                case KcpProtocalType.SYN: {
                    break;
                }

                case KcpProtocalType.ACK: {
                    if (this._channel == null) {
                        this._connectTask.SetResult(0);

                        var channel = this.CreateChannel();
                        if (channel == null)
                            throw new Exception("Create Channel Fail!");

                        channel.UserToken = this;
                        channel.OnRecvMessage = this.OnChannelRecvMessage;
                        channel.OnSendMessage = this.OnChannelSendMessage;
                        channel.OnError = this.OnChannelError;

                        channel.Init(remoteConv, remoteEndPoint, this._kcpArrayPool);
                        this._channel = channel;

                        Log.Info($"连接到了服务器: {remoteEndPoint}");

                        foreach (var plug in this.ForeachPlugs<IOnChannelStarted>()) {
                            try {
                                plug.Handle(this._channel.ChannelId);
                            }
                            catch (Exception e) {
                                Log.Error(e);
                            }
                        }
                    }

                    break;
                }

                case KcpProtocalType.FIN: {
                    this.Disconnect();
                    break;
                }

                case KcpProtocalType.MSG: {
                    var channel = this._channel;
                    if (channel == null) {
                        this.SendFINTo(remoteConv, ErrorCode.Error_Disconnect, remoteEndPoint);
                        break;
                    }

                    channel.Read(data);
                    break;
                }
            }

            this.OnChannelRecvData(remoteConv, data);
        }

        private void OnSendToData(EndPoint remoteEndPoint, Memory<byte> data) {
            var span = data.Span;
            var head = span[0];
            var remoteConv = BitConverter.ToUInt32(data.Slice(1).Span);
            this.OnChannelSendData(remoteConv, data);
        }

        public override Channel GetChannel(long channelId) {
            return this._channel;
        }

        private KcpChannel CreateChannel() {
            this.Container.StartStage();
            var channel = this.Container.Resolve<KcpChannel>();
            this.Container.EndStage();
            return channel;
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

            this._connector.StartSendToAsync();
        }

        private void SendSYNTo(uint localConv, EndPoint remoteEndPoint) {
            this._sendToCache[0] = KcpProtocalType.SYN;
            this._sendToCache.WriteTo(1, localConv);
            this.SendTo(this._sendToCache, 0, 5, remoteEndPoint);
        }

        private void SendFINTo(uint localConv, int errorId, EndPoint remoteEndPoint) {
            this._sendToCache[0] = KcpProtocalType.FIN;
            this._sendToCache.WriteTo(1, localConv);
            this._sendToCache.WriteTo(5, errorId);
            this.SendTo(this._sendToCache, 0, 9, remoteEndPoint);
        }

        private void SendTo(byte[] data, int offset, int count, EndPoint remoteEndPoint) {
            var ret = this._connector.SendToAsync(data, offset, count, remoteEndPoint);
            if (!ret) {
                Log.Error("Client发送积压测试");
            }
        }

        public void Update(uint currentTimeMS) {
            return;
            if (this._channel != null) {
                this._channel.Update(currentTimeMS);
            }
        }

        private void KcpDriver() {
            while (true) {
                // break;
                if (this._connector == null)
                    break;

                if (this._channel == null) {
                    continue;
                }

                var currentTime = TimeInfo.CurrentUtcMilliseconds;
                this._channel.Update(currentTime);

                Thread.Sleep(0);
            }
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

                    Log.Info($"客户端{this._connector.Socket.LocalEndPoint}断开连接");
                    this.SendFINTo((uint)this._channel.ChannelId, ErrorCode.Error_Disconnect, this._channel.RemoteEndPoint);
                    this._channel.Dispose();
                    this._channel = null;
                }
            }
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
                this._recvBuffer.Dispose();
                this._kcpThread.Abort();
                this._kcpThread = null;
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
            }

            public void Dispose() {
                this.Reset();
            }
        }
    }
}