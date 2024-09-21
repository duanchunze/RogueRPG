using System;
using System.Collections;
using System.Collections.Generic;

namespace Hsenl.Network {
    // 检测用户是否还活跃, 长时间不活跃的话, 就踢下线
    public class KeepalivePlug : IPlug, IOnChannelStarted {
        private Acceptor _acceptor;
        private TrafficMonitoringPlug _trafficMonitoring;
        private Queue<(long tillTime, long channelId, int bytesRecv)> _timers = new();

        private long _lowestTillTime;
        private int _corId;
        private int _intervalSeconds;

        public KeepalivePlug(int intervalSeconds = 60) {
            this._intervalSeconds = intervalSeconds;
        }

        public void Init(IPluggable pluggable) {
            if (pluggable is not Acceptor server) {
                Log.Error($"KeepalivePlug only using on Server! '{pluggable.GetType()}'");
                return;
            }

            this._acceptor = server;

            this._trafficMonitoring = pluggable.GetPlugOfType<TrafficMonitoringPlug>();
            if (this._trafficMonitoring == null) {
                Log.Error("Use KeepalivePlug must have TrafficMonitoringPlug");
                return;
            }

            this._corId = Coroutine.Start(this.LoopCheck());
        }

        public void Dispose() {
            this._acceptor = null;
            this._trafficMonitoring = null;
            this._timers.Clear();
            this._timers = null;
            this._lowestTillTime = 0;
            this._intervalSeconds = 0;
            Coroutine.Stop(this._corId);
            this._corId = 0;
        }

        public void Handle(long channelId) {
            try {
                this.Start(channelId);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void Start(long channelId) {
            if (channelId == 0)
                Log.Error("channel id is 0, cant start KeepalivePlug!");

            if (!this.GetBytesRecv(channelId, out var bytesRecv)) {
                return;
            }

            var tillTime = DateTime.Now.Ticks + this._intervalSeconds * 1000 * 10000;
            if (this._timers.Count == 0)
                this._lowestTillTime = tillTime;

            this._timers.Enqueue((tillTime, channelId, bytesRecv));
        }

        private IEnumerator LoopCheck() {
            while (this._acceptor != null) {
                if (this._timers.Count != 0) {
                    if (DateTime.Now.Ticks > this._lowestTillTime) {
                        var tuple = this._timers.Dequeue();

                        if (!this.GetBytesRecv(tuple.channelId, out var nowBytesRecv)) {
                            goto FLGA; // 代表该channel已经不存在了, 或者是我们plug本身有问题
                        }

                        // Log.Error($"检测数据变化: {tuple.bytesRecv} - {nowBytesRecv}");
                        if (tuple.bytesRecv == nowBytesRecv) {
                            // 接收数据没有变化, 则断开他
                            this.DisconnectChannel(tuple.channelId);
                            goto FLGA;
                        }

                        tuple.bytesRecv = nowBytesRecv;

                        var tillTime = tuple.tillTime + this._intervalSeconds * 1000 * 10000;
                        tuple.tillTime = tillTime;
                        this._timers.Enqueue(tuple);

                        FLGA:
                        if (this._timers.Count != 0)
                            this._lowestTillTime = this._timers.Peek().tillTime;
                        continue;
                    }
                }

                yield return null;
            }
        }

        private bool GetBytesRecv(long channelId, out int bytesRecv) {
            bytesRecv = 0;
            if (this._trafficMonitoring == null)
                return false;

            bytesRecv = this._trafficMonitoring.GetTraffic(channelId).tbr;
            return true;
        }

        private void DisconnectChannel(long channelId) {
            this._acceptor.DisconnectChannel(channelId);
        }
    }
}