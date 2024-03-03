using System;
using System.Net.Sockets;

namespace Hsenl.Network {
    /*
     * SocketAsyncEventArgs内部并没有开启多线程, SocketAsyncEventArgs之所以能实现单线程的并发操作, 因为他把请求交给了外部网络子系统去处理, 自己就呆在主线程里等消息.
     * 而Socket的其他方案, 如同步或者异步方案, 都是把请求放在自己的主线程里进行处理.
     *
     * SocketAsyncEventArgs接收数据的时候, 发生了以下几次复制
     * 1、当数据通过网络到达时, 操作系统的TCP/IP栈会将数据从网络接口卡(NIC)接收并缓存到内核缓冲区
     * 2、当ReceiveAsync方法被调用时, 操作系统会将数据从内核缓冲区复制到用户空间的缓冲区, 这个缓冲区就是EventArgs的Buffer或BufferList
     * 3、最后就是我们使用这些数据时, 可能需要把数据从Buffer中拷贝出来
     */
    public class IOCPService : Service {
        private Socket _listener;
        private SocketAsyncEventArgs _listenerEventArgs; // 服务器自己的event args, 用于接受连接

        public Container Container { get; private set; } = new();

        private readonly object _closingLocker = new();

        public IOCPService() {
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

        public override void Start() {
            if (this._listener != null)
                throw new Exception("Service is already start");

            var localEndPoint = this.Config.GetListenIPEndPoint();
            if (localEndPoint == null)
                throw new Exception($"Listen IPEndPoint Invalid!");

            this._listener = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            this._listener.Bind(localEndPoint);
            this._listener.Listen(this.Config.Backlog);

            this._listenerEventArgs = new SocketAsyncEventArgs();
            this._listenerEventArgs.Completed += this.AcceptEventArg_Completed;
            this.StartAccept(this._listenerEventArgs);

            Log.Info($"IOCP_Tcp服务器启动成功...{localEndPoint}");
        }

        // 接受客户端连接
        private void StartAccept(SocketAsyncEventArgs e) {
            e.AcceptSocket = null;
            if (!this._listener.AcceptAsync(e)) {
                this.ProcessAccept(e);
            }
        }

        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e) {
            try {
                // 处理这个连接请求
                this.ProcessAccept(e);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }

            // 继续接受其他客户端连接
            this.StartAccept(e);
        }

        private void ProcessAccept(SocketAsyncEventArgs e) {
            if (e.LastOperation != SocketAsyncOperation.Accept)
                return;

            // 进来了一个新的客户端, 给他分配一个专门的通道
            var channel = this.TryRentChannel<IChannel>(out var result) ? result : this.ChannelCreateFunc?.Invoke();

            if (channel == null)
                throw new Exception("Get Channel Fail!");

            var uniqueId = SnowflakeIdGenerator.GenerateId();
            channel.ChannelId = uniqueId;

            this.AddChannel(channel);

            // 启动通道
            channel.Start(e.AcceptSocket);

            this.Foreach<IOnChannelStarted>(started => { started.Handle(channel.ChannelId); });

            Log.Info($"接受客户端'{e.AcceptSocket.RemoteEndPoint}'的连接! 现在有{this.GetChannels().Count}个客户端在连接服务器");
        }

        public override void Write(long channelId, Func<PackageBuffer, ushort> func) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return;
            }

            channel.Write(func);
        }

        public override bool Send(long channelId) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return false;
            }

            return channel.Send();
        }

        public override void DisconnectChannel(long channelId) {
            lock (this._closingLocker) {
                if (this.IsClosed)
                    return;

                base.DisconnectChannel(channelId);
            }
        }

        public override void Close() {
            lock (this._closingLocker) {
                if (this.IsClosed)
                    return;

                base.Close();

                // 服务器的listener只需要close, 不需要shutdown, 因为服务器的listener压根没有I/O操作, 所以谈不上要shutdown, shutdown反而会报错
                this._listenerEventArgs.Completed -= this.AcceptEventArg_Completed;
                this._listenerEventArgs.Dispose();
                this._listenerEventArgs = null;
                this._listener.Close();
                this._listener.Dispose();
                this._listener = null;
                this.Container.Dispose();
                this.Container = null;
            }
        }
    }
}