using System;
using System.Net.Sockets;

namespace Hsenl.Network.Client {
    public class IOCPClient : Client {
        private Socket _connecter;
        private SocketAsyncEventArgs _connecterEventArgs;

        private HTask? _startHTask;

        public Container Container { get; private set; } = new();

        private readonly object _closingLocker = new();

        public IOCPClient() {
            this.Container.Register<IOCPChannel>().As<IChannel>().AllowAutoInjection();
            this.Container.Register<IOCPPackageReceiver>().As<IPackageRecvBufferProvider>().ShareInjection();
            this.Container.Register<IOCPPackageSender>().As<IPackageSendBufferProvider>().ShareInjection();
            this.Container.Register<IOCPPackageReceiver>().As<IPackageReader>().ShareInjection();
            this.Container.Register<IOCPPackageSender>().As<IPackageWriter>().ShareInjection();

            this.SetChannelCreateFunc(() => {
                this.Container.StartStage();
                var channel = this.Container.Resolve<IChannel>();
                this.Container.EndStage();
                channel.Init(this.Config.RecvBufferSize, this.Config.SendBufferSize);
                return channel;
            });
        }

        public override bool IsConnecting => this.GetChannel() != null;

        public override void Start() {
            var remoteEndPoint = this.Config.GetRemoteIPEndPoint();
            if (remoteEndPoint == null)
                throw new Exception("Remote IPEndPoint Invalid!");

            this._connecter = new Socket(remoteEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._connecterEventArgs = new SocketAsyncEventArgs();
            this._connecterEventArgs.Completed += this.ConnecterEventArg_OnCompleted;
            this._connecterEventArgs.RemoteEndPoint = remoteEndPoint;
            if (!this._connecter.ConnectAsync(this._connecterEventArgs)) {
                this.ProcessConnect(this._connecterEventArgs);
            }
        }

        public override async HTask StartAsync() {
            this._startHTask?.Abort();
            this._startHTask = HTask.Create();
            this.Start();
            await this._startHTask.Value;
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

            var channel = this.ChannelCreateFunc?.Invoke();
            if (channel == null)
                throw new Exception("Get Channel Fail!");

            this.SetChannel(channel);

            // 启动通道
            channel.Start(e.ConnectSocket);

            Log.Info($"连接到了服务器: {channel.Socket.RemoteEndPoint}");

            lock (this._closingLocker) {
                if (this._startHTask != null) {
                    var temp = this._startHTask.Value;
                    this._startHTask = null;
                    temp.SetResult();
                }
            }
        }

        public override void Write(Func<PackageBuffer, ushort> func) {
            var channel = this.GetChannel();
            if (channel == null) {
                Log.Error("Client is not connected, can't wirte data");
                return;
            }

            channel.Write(func);
        }

        public override bool Send() {
            var channel = this.GetChannel();
            if (channel == null) {
                Log.Error("Client is not connected, can't wirte data");
                return false;
            }

            return channel.Send();
        }

        public override void Disconnect() {
            lock (this._closingLocker) {
                if (this.IsClosed)
                    return;

                base.Disconnect();

                this._connecterEventArgs.Completed -= this.ConnecterEventArg_OnCompleted;
                this._connecter.Close();
                if (this._startHTask != null) {
                    this._startHTask.Value.Abort();
                    this._startHTask = null;
                }

                this.Foreach<IOnChannelDisconnected>(started => { started.Handle(0); });
            }
        }

        public override void Close() {
            lock (this._closingLocker) {
                if (this.IsClosed)
                    return;

                this.DisconnectChannel();
                base.Close();

                this._connecterEventArgs.Completed -= this.ConnecterEventArg_OnCompleted;
                this._connecterEventArgs.Dispose();
                this._connecterEventArgs = null;
                this._connecter.Close();
                this._connecter.Dispose();
                this._connecter = null;
                if (this._startHTask != null) {
                    this._startHTask.Value.Abort();
                    this._startHTask = null;
                }

                this.Container.Dispose();
                this.Container = null;
            }
        }
    }
}