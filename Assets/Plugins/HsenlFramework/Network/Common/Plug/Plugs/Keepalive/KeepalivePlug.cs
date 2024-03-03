using System;
using System.Collections;
using System.Collections.Generic;

namespace Hsenl.Network {
    public class KeepalivePlug : IPlug, IOnChannelStarted {
        private IPluggable _pluggable;
        private Queue<(long tillTime, long channelId, int bytesRecv)> timers = new();

        private long _lowestTillTime;
        private int _corId;
        private const int intervalSeconds = 10;

        public void Init(IPluggable pluggable) {
            if (pluggable is not IService)
                Log.Error($"KeepalivePlug only using on service! '{pluggable.GetType()}'");

            this._pluggable = pluggable;

            this._corId = Coroutine.Start(this.LoopCheck());
        }

        public void Dispose() {
            this._pluggable = null;
            this.timers.Clear();
            this.timers = null;
            this._lowestTillTime = 0;
            Coroutine.Stop(this._corId);
            this._corId = 0;
        }

        private void Start(long channelId) {
            if (this._pluggable == null)
                return;

            if (channelId == 0)
                Log.Error("channel id is 0, cant start KeepalivePlug!");

            var bytesRecv = this.GetBytesRecv(channelId);
            if (bytesRecv == -1)
                return;

            var tillTime = DateTime.Now.Ticks + intervalSeconds * 1000 * 10000;
            if (this.timers.Count == 0)
                this._lowestTillTime = tillTime;

            this.timers.Enqueue((tillTime, channelId, bytesRecv));
        }

        public void Handle(long channelId) {
            try {
                this.Start(channelId);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private IEnumerator LoopCheck() {
            while (this._pluggable != null) {
                if (this.timers.Count != 0) {
                    if (DateTime.Now.Ticks > this._lowestTillTime) {
                        var tuple = this.timers.Dequeue();

                        int nowBytesRecv = this.GetBytesRecv(tuple.channelId);
                        if (nowBytesRecv == -1) // -1代表该channel已经不存在了, 或者是我们plug本身有问题
                            goto FLGA;

                        if (tuple.bytesRecv == nowBytesRecv) {
                            // 接收数据没有变化, 则断开他
                            this.DisconnectChannel(tuple.channelId);
                            goto FLGA;
                        }

                        tuple.bytesRecv = nowBytesRecv;

                        var tillTime = tuple.tillTime + intervalSeconds * 1000 * 10000;
                        tuple.tillTime = tillTime;
                        this.timers.Enqueue(tuple);

                        FLGA:
                        if (this.timers.Count != 0)
                            this._lowestTillTime = this.timers.Peek().tillTime;
                        continue;
                    }
                }

                yield return null;
            }
        }

        private int GetBytesRecv(long channelId) {
            if (this._pluggable == null)
                return -1;

            if (this._pluggable is IService service) {
                return service.GetTotalBytesRecvOfChannel(channelId);
            }

            return -1;
        }

        private void DisconnectChannel(long channelId) {
            if (this._pluggable is IService service) {
                service.DisconnectChannel(channelId);
            }
        }
    }
}