using System;
using System.Collections.Generic;
using System.Threading;

namespace Hsenl.Network.Client {
    public abstract class Client : IClient, IPluggable {
        private ClientConfig _config;

        private IChannel _channel;

        protected int totalBytesRecv; // 记录总共读取字节数
        protected int totalBytesSend; // 记录总共发送字节数

        public Func<IChannel> ChannelCreateFunc { get; private set; }

        public Action OnConnected { get; set; }
        public Action OnDisconnected { get; set; }
        public Action<Memory<byte>> OnRecvPackage { get; set; }
        public Action<ushort, Memory<byte>> OnRecvMessage { get; set; }

        private bool _isClosed;

        private Dictionary<Type, Dictionary<Type, IPlug>> Plugs { get; set; } = new();

        public ClientConfig Config {
            get => this._config;
            set => this._config = value;
        }

        public int TotalBytesRecv => this.totalBytesRecv;

        public int TotalBytesSend => this.totalBytesSend;

        public bool IsClosed {
            get => this._isClosed;
            protected set => this._isClosed = value;
        }

        public abstract bool IsConnecting { get; }

        public void SetChannelCreateFunc(Func<IChannel> func) {
            this.ChannelCreateFunc = func;
        }

        public abstract void Start();

        public abstract HTask StartAsync();

        public abstract void Write(Func<PackageBuffer, ushort> func);

        public abstract bool Send();

        protected IChannel GetChannel() {
            return this._channel;
        }

        protected virtual void SetChannel(IChannel channel) {
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            this._channel = channel;

            try {
                this.OnConnected?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            channel.OnRecvPackage += (_, data) => { this.OnChannelRecvPackage(data); };
            channel.OnRecvMessage += (_, opcode, data) => { this.OnChannelRecvMessage(opcode, data); };
            channel.OnSendPackage += (_, data) => { this.OnChannelSendPackage(data); };
            channel.OnSendMessage += (_, opcode, data) => { this.OnChannelSendMessage(opcode, data); };
            channel.OnError += (_, errorCode) => { this.OnChannelError(errorCode); };
        }

        protected virtual void OnChannelRecvPackage(Memory<byte> data) {
            Interlocked.Add(ref this.totalBytesRecv, data.Length);
            Log.Info($"客户端接收到了包:  {data.Length} b / {this.totalBytesRecv} b -- {(data.Length / 1024f):f2} kb / {(this.totalBytesRecv / 1024f):f2} kb");
            try {
                this.OnRecvPackage?.Invoke(data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelRecvMessage(ushort opcode, Memory<byte> data) {
            Log.Info($"客户端接收到了一个消息:  {data.Length} b / {this.totalBytesRecv} b -- {(data.Length / 1024f):f2} kb / {(this.totalBytesRecv / 1024f):f2} kb");
            try {
                this.Foreach<IAfterMessageReaded>(action => {
                    action.Handle(ref data); //
                });
                this.OnRecvMessage?.Invoke(opcode, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelSendPackage(Memory<byte> data) {
            Interlocked.Add(ref this.totalBytesSend, data.Length);
        }

        protected virtual void OnChannelSendMessage(ushort opcode, Memory<byte> data) {
            this.Foreach<IAfterMessageWrited>(action => {
                action.Handle(ref data); //
            });
        }

        protected virtual void OnChannelError(int errorCode) {
            this.Disconnect();
        }

        protected virtual void DisconnectChannel() {
            if (this._channel == null)
                return;

            try {
                this._channel.Dispose();
                this._channel = null;
                Log.Info("从服务器断开");
                try {
                    this.OnDisconnected?.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public virtual void Disconnect() {
            if (this.IsClosed)
                throw new Exception("Client is closed!");

            this.DisconnectChannel();
        }

        public virtual void Close() {
            if (this.IsClosed)
                return;

            this.IsClosed = true;
            this._config = default;
            this.totalBytesRecv = 0;
            this.totalBytesSend = 0;
            this.ChannelCreateFunc = null;
            this.OnConnected = null;
            this.OnDisconnected = null;
            this.OnRecvPackage = null;
            this.OnRecvMessage = null;
            this.Plugs?.Clear();
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