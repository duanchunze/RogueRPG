using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Hsenl.Network {
    public abstract class Service : IService, IPluggable {
        private ServiceConfig _config;

        private ConcurrentDictionary<long, IChannel> _channels = new();
        private NetworkPool<IChannel> _channelPool = new();

        protected int totalBytesRecv; // 记录总共读取字节数
        protected int totalBytesSend; // 记录总共发送字节数

        protected Func<IChannel> ChannelCreateFunc { get; private set; }

        private Dictionary<Type, Dictionary<Type, IPlug>> Plugs { get; set; } = new();

        public Action<long> OnConnected { get; set; }
        public Action<long> OnDisconnected { get; set; }
        public Action<long, Memory<byte>> OnRecvPackage { get; set; }
        public Action<long, ushort, Memory<byte>> OnRecvMessage { get; set; }

        private bool _isClosed;

        public ServiceConfig Config {
            get => this._config;
            set => this._config = value;
        }

        public int TotalBytesRecv => this.totalBytesRecv;

        public int TotalBytesSend => this.totalBytesSend;

        public bool IsClosed {
            get => this._isClosed;
            protected set => this._isClosed = value;
        }

        public int GetTotalBytesRecvOfChannel(long channelId) {
            var channel = this.GetChannel(channelId);
            if (channel == null)
                return -1;

            return channel.TotalBytesRecv;
        }

        public int GetTotalBytesSendOfChannel(long channelId) {
            var channel = this.GetChannel(channelId);
            if (channel == null)
                return 0;

            return channel.TotalBytesSend;
        }

        protected bool TryRentChannel<T>(out T channel) {
            if (this._channelPool.TryRent(out var result)) {
                channel = (T)result;
                return true;
            }

            channel = default;
            return false;
        }

        protected void ReturnChannel(IChannel channel) {
            this._channelPool.Return(channel);
        }

        public void SetChannelCreateFunc(Func<IChannel> func) {
            this.ChannelCreateFunc = func;
        }

        public abstract void Start();

        public abstract void Write(long channelId, Func<PackageBuffer, ushort> func);

        public abstract bool Send(long channelId);

        protected IChannel GetChannel(long channelId) {
            this._channels.TryGetValue(channelId, out var channel);
            return channel;
        }

        protected ICollection<IChannel> GetChannels() {
            return this._channels.Values;
        }

        protected virtual void AddChannel(IChannel channel) {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            try {
                this._channels.TryAdd(channel.ChannelId, channel);

                try {
                    this.OnConnected?.Invoke(channel.ChannelId);
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                channel.OnRecvPackage += this.OnChannelRecvPackage;
                channel.OnRecvMessage += this.OnChannelRecvMessage;
                channel.OnSendPackage += this.OnChannelSendPackage;
                channel.OnSendMessage += this.OnChannelSendMessage;
                channel.OnError += this.OnChannelError;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelRecvPackage(long channelId, Memory<byte> data) {
            Interlocked.Add(ref this.totalBytesRecv, data.Length);
            Log.Info($"服务器接收到了包:  {data.Length} b / {this.totalBytesRecv} b -- {(data.Length / 1024f):f2} kb / {(this.totalBytesRecv / 1024f):f2} kb");
            try {
                this.OnRecvPackage?.Invoke(channelId, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelRecvMessage(long channelId, ushort opcode, Memory<byte> data) {
            Log.Info($"服务器接收到了一个消息:  {data.Length} b / {this.totalBytesRecv} b -- {(data.Length / 1024f):f2} kb / {(this.totalBytesRecv / 1024f):f2} kb");
            try {
                this.Foreach<IAfterMessageReaded>(action => {
                    action.Handle(ref data); //
                });
                this.OnRecvMessage?.Invoke(channelId, opcode, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelSendPackage(long channelId, Memory<byte> data) {
            Interlocked.Add(ref this.totalBytesSend, data.Length);
        }

        protected virtual void OnChannelSendMessage(long channelId, ushort opcode, Memory<byte> data) {
            this.Foreach<IAfterMessageWrited>(action => {
                action.Handle(ref data); //
            });
        }

        protected virtual void OnChannelError(long channelId, int errorCode) {
            this.DisconnectChannel(channelId);
        }

        public virtual void DisconnectChannel(long channelId) {
            if (this.IsClosed)
                throw new Exception("Service is closed!");

            if (this._channels.TryRemove(channelId, out var channel)) {
                Log.Info($"一个客户端从服务器断开 '{channel.Socket?.RemoteEndPoint}'! 现在有{{0}}个客户端在连接服务器", this._channels?.Count ?? 0);
                channel.Disconnect();
                this._channelPool.Return(channel);
                
                try {
                    this.OnDisconnected?.Invoke(channelId);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public virtual void Close() {
            if (this.IsClosed)
                return;

            this.IsClosed = true;

            foreach (var channel in this._channels.Values) {
                try {
                    channel.Dispose();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            this._config.Reset();
            this._config = null;
            this._channels.Clear();
            this._channels = null;
            this._channelPool.Clear();
            this._channelPool = null;
            this.OnConnected = null;
            this.OnDisconnected = null;
            this.OnRecvPackage = null;
            this.OnRecvMessage = null;
            this.totalBytesRecv = 0;
            this.totalBytesSend = 0;
            this.Plugs.Clear();
            this.Plugs = null;
        }

        #region Pluggable Functions

        public Tg[] GetPlugsOfGroup<Tg>() where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            Tg[] array;
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                array = new Tg[dict.Values.Count];
                int index = 0;
                foreach (var value in dict.Values) {
                    array[index++] = (Tg)value;
                }
            }

            array = Array.Empty<Tg>();
            return array;
        }

        public bool GetPlugsOfGroup<Tg>(List<Tg> plugs) where Tg : IPlugGroup {
            if (plugs == null)
                throw new ArgumentNullException(nameof(plugs));

            var groupType = typeof(Tg);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                foreach (var kv in dict) {
                    plugs.Add((Tg)kv.Value);
                }

                return true;
            }

            return false;
        }

        public T GetPlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            var plugType = typeof(T);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                if (dict.TryGetValue(plugType, out var plug))
                    return (T)plug;
            }

            return default;
        }

        public void Foreach<Tg>(Action<Tg> action) where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                foreach (var kv in dict) {
                    action.Invoke((Tg)kv.Value);
                }
            }
        }

        public void AddPlug<Tg, T>(T plug) where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            if (!this.Plugs.TryGetValue(groupType, out var dict)) {
                dict = new Dictionary<Type, IPlug>();
                this.Plugs[groupType] = dict;
            }

            dict[plug.GetType()] = plug;
            try {
                plug.Init(this);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public bool RemovePlug<Tg, T>() where Tg : IPlugGroup where T : IPlug, IPlugGroup {
            var groupType = typeof(Tg);
            var plugType = typeof(T);
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                if (!dict.Remove(plugType, out var plug)) {
                    return false;
                }

                if (dict.Count == 0)
                    this.Plugs.Remove(groupType);

                try {
                    plug.Dispose();
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                return true;
            }

            return false;
        }

        public bool RemovePlug<Tg>() where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            if (!this.Plugs.Remove(groupType, out var dict)) {
                return false;
            }

            foreach (var kv in dict) {
                try {
                    kv.Value.Dispose();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            return true;
        }

        #endregion
    }
}