using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

// ReSharper disable InconsistentlySynchronizedField

namespace Hsenl.Network {
    public sealed class TcpServer : Acceptor {
        private TcpAcceptor _acceptor;
        private readonly ConcurrentDictionary<long, TcpChannel> _channels = new();
        private readonly NetworkPool<TcpChannel> _channelPool = new();
        public Configure Config { get; private set; }
        public Action<long> OnConnected { get; set; }
        public Action<long> OnDisconnected { get; set; }
        public Container Container { get; }
        private readonly object _locker = new();

        public TcpServer(Configure config) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));

            this.Container = new Container();
            this.Container.Let<TcpChannel>().AllowAutoInjection();
            this.Container.Let<TcpPacketReceiver>().As<IPacketRecvProvider>().ShareInjection();
            this.Container.Let<TcpPacketSender>().As<IPacketSendProvider>().ShareInjection();
            this.Container.Let<TcpPacketReceiver>().As<IMessageReader>().ShareInjection();
            this.Container.Let<TcpPacketSender>().As<IMessageWriter>().ShareInjection();
        }

        public TcpServer(Configure config, Container container) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));
            this.Container = container ?? throw new ArgumentNullException(nameof(container));
        }

        public override void StartAccept() {
            this.CheckDisposedException();

            if (this._acceptor != null)
                throw new Exception("Service is already start");

            var localEndPoint = this.Config.GetListenIPEndPoint();
            if (localEndPoint == null)
                throw new Exception($"Listen IPEndPoint Invalid!");

            this._acceptor = new TcpAcceptor(localEndPoint);
            this._acceptor.Socket.Listen(this.Config.Backlog);
            this._acceptor.OnAccepted = this.ProcessAccept;
            this._acceptor.StartAccept();

            Log.Info($"Tcp服务器启动成功...{localEndPoint}");
        }

        private void ProcessAccept(Socket acceptSocket) {
            // 进来了一个新的客户端, 给他分配一个专门的通道
            var channel = this.TryRentChannel(out var result) ? result : this.CreateChannel();

            if (channel == null)
                throw new Exception("Create Channel Fail!");

            var socket = new TcpSocket(acceptSocket) {
                RecvBufferSize = this.Config.RecvBufferSize,
                SendBufferSize = this.Config.SendBufferSize
            };
            channel.UserToken = socket;
            channel.OnRecvMessage = this.OnChannelRecvMessage;
            channel.OnSendMessage = this.OnChannelSendMessage;
            channel.OnError = this.OnChannelError;
            var channelId = SnowflakeIdGenerator.GenerateId();
            channel.Init(channelId);
            this._channels.TryAdd(channel.ChannelId, channel);

            socket.RecvBufferGetter = channel.GetRecvBuffer;
            socket.SendBufferGetter = channel.GetSendBuffer;
            socket.OnRecvData = data => { this.OnChannelRecvData(channel.ChannelId, data); };
            socket.OnSendData = data => { this.OnChannelSendData(channel.ChannelId, data); };
            socket.StartReceiveAsync();

            try {
                this.OnConnected?.Invoke(channel.ChannelId);
            }
            catch (Exception ex) {
                Log.Error(ex);
            }

            foreach (var plug in this.ForeachPlugs<IOnChannelStarted>()) {
                try {
                    plug.Handle(channel.ChannelId);
                }
                catch (Exception ex) {
                    Log.Error(ex);
                }
            }

            Log.Info($"接受客户端'{acceptSocket.RemoteEndPoint}'的连接! 现在有{this._channels.Count}个客户端在连接服务器");
        }

        private bool TryRentChannel(out TcpChannel channel) {
            if (this._channelPool.TryRent(out var result)) {
                channel = result;
                return true;
            }

            channel = default;
            return false;
        }

        private void ReturnChannel(TcpChannel channel) {
            this._channelPool.Return(channel);
        }

        private TcpChannel CreateChannel() {
            this.Container.StartStage();
            var channel = this.Container.Resolve<TcpChannel>();
            this.Container.EndStage();
            return channel;
        }

        public override Channel GetChannel(long channelId) {
            this._channels.TryGetValue(channelId, out var channel);
            return channel;
        }

        protected override void OnChannelRecvData(long channelId, Memory<byte> data) {
            if (this._channels.TryGetValue(channelId, out var channel)) {
                channel.Read(data);
            }

            base.OnChannelRecvData(channelId, data);
        }

        protected override void OnChannelError(long channelId, int errorCode) {
            base.OnChannelError(channelId, errorCode);
            if (errorCode >= ErrorCode.SocketError)
                return;

            this.DisconnectChannel(channelId);
        }

        public override void Write(long channelId, byte[] data, int offset, int count) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return;
            }

            channel.Write(data, offset, count);
        }

        public override void Write(long channelId, Span<byte> data) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return;
            }

            channel.Write(data);
        }

        public override void StartSend(long channelId) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return;
            }

            ((TcpSocket)channel.UserToken).StartSendAsync();
        }

        public bool Send(long channelId, IList<ArraySegment<byte>> buffers) {
            var channel = this.GetChannel(channelId);
            if (channel == null) {
                Log.Error("Channel is not exist!");
                return false;
            }

            return ((TcpSocket)channel.UserToken).SendPacketsAsync(buffers);
        }

        public override void DisconnectChannel(long channelId) {
            lock (this._locker) {
                if (this._channels.ContainsKey(channelId)) {
                    foreach (var plug in this.ForeachPlugs<IOnChannelDisconnect>()) {
                        try {
                            plug.Handle(channelId);
                        }
                        catch (Exception e) {
                            Log.Error(e);
                        }
                    }

                    this._channels.TryRemove(channelId, out var channel);

                    Log.Info($"一个客户端从服务器断开 '{((TcpSocket)channel.UserToken).Socket.RemoteEndPoint}'! 现在有{0}个客户端在连接服务器", this._channels?.Count ?? 0);
                    (channel.UserToken as TcpSocket)?.Dispose();
                    channel.Dispose();
                    this.ReturnChannel(channel);

                    try {
                        this.OnDisconnected?.Invoke(channelId);
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }
                }
            }
        }

        public void DisconnectAllChannels() {
            lock (this._locker) {
                foreach (var kv in this._channels) {
                    var channel = kv.Value;
                    foreach (var plug in this.ForeachPlugs<IOnChannelDisconnect>()) {
                        try {
                            plug.Handle(channel.ChannelId);
                        }
                        catch (Exception e) {
                            Log.Error(e);
                        }
                    }

                    (channel.UserToken as TcpSocket)?.Dispose();
                    channel.Dispose();
                    this.ReturnChannel(channel);

                    try {
                        this.OnDisconnected?.Invoke(channel.ChannelId);
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }
                }

                this._channels.Clear();
            }
        }

        public override void Dispose() {
            if (this.IsDisposed)
                return;

            base.Dispose();

            this.DisconnectAllChannels();

            this.Config.Dispose();
            this.Config = null;
            this._acceptor.Dispose();
            this._acceptor = null;
            this._channels.Clear();
            this._channelPool.Clear();
            this.OnConnected = null;
            this.OnDisconnected = null;
            this.Container.Dispose();
        }

        public class Configure {
            public string LocalIPHost { get; set; }
            public int Port { get; set; }
            public int Backlog { get; set; }

            public int RecvBufferSize { get; set; }

            public int SendBufferSize { get; set; }

            public IPEndPoint GetListenIPEndPoint() {
                if (IPAddress.TryParse(this.LocalIPHost, out var address)) {
                    var endPoint = new IPEndPoint(address, this.Port);
                    return endPoint;
                }

                return null;
            }

            public void Reset() {
                this.LocalIPHost = null;
                this.Port = 0;
                this.Backlog = 0;
                this.RecvBufferSize = 0;
                this.SendBufferSize = 0;
            }

            public void Dispose() {
                this.Reset();
            }
        }
    }
}