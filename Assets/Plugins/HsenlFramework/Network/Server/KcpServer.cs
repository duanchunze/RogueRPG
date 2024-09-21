using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;

// ReSharper disable InconsistentlySynchronizedField

namespace Hsenl.Network {
    public sealed class KcpServer : Acceptor {
        private UdpSocket _acceptor;
        private readonly ConcurrentDictionary<long, KcpChannel> _channels = new();
        private readonly NetworkPool<KcpChannel> _channelPool = new();

        public Configure Config { get; private set; }
        public Action<long> OnConnected { get; set; }
        public Action<long> OnDisconnected { get; set; }
        public Container Container { get; }

        private readonly HBuffer _recvBuffer = new(); // kcp所有的channel都用一个socket来实现数据收发
        private const int MaxArrayLength = 24;
        private readonly ArrayPool<byte> _sendToArrayPool = ArrayPool<byte>.Create(MaxArrayLength, 100); // 专供SendTo方法使用
        private readonly ArrayPool<byte> _kcpArrayPool = ArrayPool<byte>.Create(2048, 1000); // 专供kcp使用
        private readonly Queue<(byte[] data, int offset, int count, EndPoint remoteEndPoint)> _backlogSendTo = new(); // 积压的sendTo
        private readonly Queue<long> _waitSendChannelIds = new();
        private Thread _kcpThread;
        private Queue<long> _kcpUpdateTimers = new();
        private uint _kcpUpdateMinTime;
        private readonly object _locker = new();

        public KcpServer(Configure config) {
            this.Config = config ?? throw new ArgumentNullException(nameof(config));

            this.Container = new Container();
            this.Container.Let<KcpChannel>().AllowAutoInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketHandler>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IKcpHandler>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketRecvProvider>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IPacketSendProvider>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IMessageReader>().ShareInjection();
            this.Container.Let<KcpPacketHandler>().As<IMessageWriter>().ShareInjection();
        }

        public override void StartAccept() {
            this.CheckDisposedException();

            if (this._acceptor != null)
                throw new Exception("Service is already start");

            var localEndPoint = this.Config.GetLocalIPEndPoint();
            if (localEndPoint == null)
                throw new Exception($"Listen IPEndPoint Invalid!");

            this._acceptor = new UdpSocket(localEndPoint.AddressFamily) {
                RecvBufferSize = this.Config.RecvBufferSize,
                SendBufferSize = this.Config.SendBufferSize,
            };
            this._acceptor.Socket.Bind(localEndPoint);
            this._acceptor.RecvBufferGetter = len => {
                if (len > this._recvBuffer.Length)
                    this._recvBuffer.SetLength(len);

                var buffer = this._recvBuffer.GetBuffer();
                return (buffer, 0, buffer.Length);
            };
            this._acceptor.SendBufferGetter = len => {
                // 如果有积压的sendTo, 先发送
                if (this._backlogSendTo.Count != 0) {
                    var tuple = this._backlogSendTo.Dequeue();
                    this._sendToArrayPool.Return(tuple.data);
                    return tuple;
                }

                // 遍历所有等待发送的通道
                while (this._waitSendChannelIds.Count != 0) {
                    var channelId = this._waitSendChannelIds.Dequeue();
                    var channel = this.GetKcpChannel(channelId);
                    if (channel == null) {
                        continue;
                    }

                    var bytes = channel.GetSendBuffer(len, out int offset, out int count);
                    if (count == 0) {
                        continue;
                    }

                    // 找不到通道, 或者通道没有要发的数据都跳过, 直到遇到一个能发的
                    return (bytes, offset, count, channel.RemoteEndPoint);
                }

                return default;
            };

            this._acceptor.OnRecvFromData = this.OnRecvFromData;
            this._acceptor.OnSendToData = this.OnSendToData;
            this._acceptor.StartReceiveFromAsync();

            this._kcpThread = new Thread(this.KcpDriver);
            this._kcpThread.Start();

            Log.Info($"Kcp服务器启动成功...{localEndPoint}");
        }

        private void OnRecvFromData(EndPoint remoteEndPoint, Memory<byte> data) {
            // 根据发来的数据, 判断是要连接, 还是发送消息
            var span = data.Span;
            var head = span[0];
            var remoteConv = BitConverter.ToUInt32(span.Slice(1));
            switch (head) {
                case KcpProtocalType.SYN: {
                    var channel = this.GetKcpChannel(remoteConv);
                    if (channel == null) {
                        channel = this.TryRentChannel(out var result) ? result : this.CreateChannel();
                        if (channel == null)
                            throw new Exception("Create Channel Fail!");

                        channel.UserToken = this;
                        channel.OnRecvMessage = this.OnChannelRecvMessage;
                        channel.OnSendMessage = this.OnChannelSendMessage;
                        channel.OnError = this.OnChannelError;
                        channel.Init(remoteConv, remoteEndPoint, this._kcpArrayPool);
                        this._channels.TryAdd(channel.ChannelId, channel);

                        this.SendACKTo(remoteConv, remoteEndPoint);

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

                        Log.Info($"接受客户端'{remoteEndPoint}'的连接! 现在有{this._channels.Count}个客户端在连接服务器");
                    }

                    break;
                }

                case KcpProtocalType.ACK: {
                    break;
                }

                case KcpProtocalType.FIN: {
                    this.DisconnectChannel(remoteConv);
                    break;
                }

                case KcpProtocalType.MSG: {
                    var channel = this.GetChannel(remoteConv);
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

        private KcpChannel CreateChannel() {
            this.Container.StartStage();
            var channel = this.Container.Resolve<KcpChannel>();
            this.Container.EndStage();
            return channel;
        }

        public override Channel GetChannel(long channelId) {
            if (this._channels.TryGetValue(channelId, out var channel)) {
                return channel;
            }

            return null;
        }

        public KcpChannel GetKcpChannel(long channelId) {
            if (this._channels.TryGetValue(channelId, out var channel)) {
                return channel;
            }

            return null;
        }

        private bool TryRentChannel(out KcpChannel channel) {
            if (this._channelPool.TryRent(out var result)) {
                channel = result;
                return true;
            }

            channel = default;
            return false;
        }

        private void ReturnChannel(KcpChannel channel) {
            this._channelPool.Return(channel);
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

            this._waitSendChannelIds.Enqueue(channelId);
            this._acceptor.StartSendToAsync();
        }

        private byte[] GetSendToBytes(int len) {
            if (len > MaxArrayLength)
                throw new ArgumentOutOfRangeException("");

            return this._sendToArrayPool.Rent(len);
        }

        private void SendACKTo(uint remoteConv, EndPoint remoteEndPoint) {
            var bytes = this.GetSendToBytes(5);
            bytes[0] = KcpProtocalType.ACK;
            bytes.WriteTo(1, remoteConv);
            this.SendTo(bytes, 0, 5, remoteEndPoint);
        }

        private void SendFINTo(uint localConv, int errorId, EndPoint remoteEndPoint) {
            var bytes = this.GetSendToBytes(9);
            bytes.WriteTo(1, localConv);
            bytes.WriteTo(5, errorId);
            this.SendTo(bytes, 0, 9, remoteEndPoint);
        }

        private void SendTo(byte[] data, int offset, int count, EndPoint remoteEndPoint) {
            var ret = this._acceptor.SendToAsync(data, offset, count, remoteEndPoint);
            if (!ret) {
                // Log.Error("Server发送积压测试");
                this._backlogSendTo.Enqueue((data, offset, count, remoteEndPoint));
            }
            else {
                this._sendToArrayPool.Return(data);
            }
        }

        public void Update(uint currentTimeMS) {
            return;
            foreach (var kv in this._channels) {
                var channel = kv.Value;
                channel.Update(currentTimeMS);
            }
        }

        private void KcpDriver() {
            while (true) {
                // break;
                if (this.IsDisposed)
                    break;

                var currentTime = TimeInfo.CurrentUtcMilliseconds;
                foreach (var kv in this._channels) {
                    var channel = kv.Value;
                    this._kcpUpdateMinTime = channel.Update(currentTime);
                }

                Thread.Sleep(5);
            }
        }

        protected override void OnChannelError(long channelId, int errorCode) {
            base.OnChannelError(channelId, errorCode);

            if (errorCode >= ErrorCode.SocketError)
                return;

            this.DisconnectChannel(channelId);
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

                    Log.Info($"一个客户端从服务器断开 '{channel.RemoteEndPoint}'! 现在有{{0}}个客户端在连接服务器", this._channels?.Count ?? 0);
                    this.SendFINTo((uint)channelId, ErrorCode.Error_Disconnect, channel.RemoteEndPoint);
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

                    this.SendFINTo((uint)channel.ChannelId, ErrorCode.Error_Disconnect, channel.RemoteEndPoint);
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
            this._recvBuffer.Dispose();
            this._backlogSendTo.Clear();
            this._waitSendChannelIds.Clear();
            this._kcpThread.Abort();
            this._kcpThread = null;
            this._kcpUpdateTimers.Clear();
        }

        public class Configure {
            public string LocalIPHost { get; set; }
            public int Port { get; set; }
            public int Backlog { get; set; }

            public int RecvBufferSize { get; set; }

            public int SendBufferSize { get; set; }

            public IPEndPoint GetLocalIPEndPoint() {
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