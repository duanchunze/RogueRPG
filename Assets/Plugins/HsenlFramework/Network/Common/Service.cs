using System;
using System.Collections;
using System.Collections.Generic;

namespace Hsenl.Network {
    // 统筹channel与socket的交互, 并管理他们
    public abstract class Service : IDisposable, IPluggable {
        private bool _isDisposed;

        public Action<long, Memory<byte>> OnRecvData { get; set; }
        public Action<long, Memory<byte>> OnSendData { get; set; }
        public Action<long, Memory<byte>> OnRecvMessage { get; set; }
        public Action<long, Memory<byte>> OnSendMessage { get; set; }
        public Action<long, int> OnError { get; set; }

        private Dictionary<Type, Dictionary<Type, IPlug>> Plugs { get; } = new();

        public bool IsDisposed => this._isDisposed;

        protected virtual void OnChannelRecvData(long channelId, Memory<byte> data) {
            foreach (var plug in this.ForeachPlugs<IOnRecvData>()) {
                try {
                    plug.Handle(channelId, ref data);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            try {
                this.OnRecvData?.Invoke(channelId, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelSendData(long channelId, Memory<byte> data) {
            foreach (var plug in this.ForeachPlugs<IOnSendData>()) {
                try {
                    plug.Handle(channelId, ref data);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            try {
                this.OnSendData?.Invoke(channelId, data);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelRecvMessage(long channelId, Memory<byte> message) {
            // Log.Info($"客户端接收到了一个消息:  {data.Length} b / {this._channel.TotalBytesRecv} b -- {(data.Length / 1024f):f2} kb / {(this._channel.TotalBytesRecv / 1024f):f2} kb");
            foreach (var plug in this.ForeachPlugs<IAfterMessageReaded>()) {
                try {
                    plug.Handle(channelId, ref message);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            try {
                this.OnRecvMessage?.Invoke(channelId, message);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelSendMessage(long channelId, Memory<byte> message) {
            foreach (var plug in this.ForeachPlugs<IAfterMessageWrited>()) {
                try {
                    plug.Handle(channelId, ref message);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }

            try {
                this.OnSendMessage?.Invoke(channelId, message);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnChannelError(long channelId, int errorCode) {
            try {
                this.OnError?.Invoke(channelId, errorCode);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        public abstract Channel GetChannel(long channelId);

        public abstract void Write(long channelId, byte[] data, int offset, int count);

        public abstract void Write(long channelId, Span<byte> data);

        public abstract void StartSend(long channelId);

        public virtual void Dispose() {
            this._isDisposed = true;
            this.OnRecvData = null;
            this.OnSendData = null;
            this.OnRecvMessage = null;
            this.OnSendMessage = null;
            this.OnError = null;
            foreach (var kv in this.Plugs) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Dispose();
                }

                kv.Value.Clear();
            }

            this.Plugs.Clear();
        }

        protected void CheckDisposedException() {
            if (this.IsDisposed)
                throw new Exception("Channel is disposed!");
        }

        #region Pluggable Functions

        public Tg[] GetPlugsOfGroup<Tg>() where Tg : IPlugGroup {
            var groupType = typeof(Tg);
            Tg[] array;
            if (this.Plugs.TryGetValue(groupType, out var dict)) {
                array = new Tg[dict.Count];
                int index = 0;
                foreach (var kv in dict) {
                    array[index++] = (Tg)kv.Value;
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

        public T GetPlugOfType<T>() where T : IPlug {
            foreach (var kv in this.Plugs) {
                foreach (var plugKV in kv.Value) {
                    if (plugKV.Value is T t)
                        return t;
                }
            }

            return default;
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

        public Enumerable<Tg> ForeachPlugs<Tg>() {
            var type = typeof(Tg);
            if (this.Plugs.TryGetValue(type, out var dict)) {
                return new Enumerable<Tg>(dict);
            }

            return default;
        }

        public readonly struct Enumerable<T> : IEnumerable<T> {
            private readonly Dictionary<Type, IPlug> _dict;

            public Enumerable(Dictionary<Type, IPlug> dict) {
                this._dict = dict;
            }

            public Enumerator GetEnumerator() => new Enumerator(this._dict);
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

            public struct Enumerator : IEnumerator<T> {
                private bool _null;
                private Dictionary<Type, IPlug>.Enumerator _enumerator;

                public T Current {
                    get {
                        if (this._null)
                            return default;

                        return (T)this._enumerator.Current.Value;
                    }
                }

                public Enumerator(Dictionary<Type, IPlug> dict) {
                    if (dict == null) {
                        this._null = true;
                        this._enumerator = default;
                        return;
                    }

                    this._null = false;
                    this._enumerator = dict.GetEnumerator();
                }

                object IEnumerator.Current => this.Current;

                public bool MoveNext() {
                    if (this._null)
                        return false;

                    return this._enumerator.MoveNext();
                }

                public void Reset() {
                    this._null = true;
                    this._enumerator = default;
                }

                public void Dispose() {
                    this.Reset();
                }
            }
        }

        #endregion
    }
}