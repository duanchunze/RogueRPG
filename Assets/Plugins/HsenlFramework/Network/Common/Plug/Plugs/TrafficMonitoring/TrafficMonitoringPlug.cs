using System;
using System.Collections.Generic;
using System.Net;

namespace Hsenl.Network {
    // 流量监控. 每个channel发送与接收了多少数据
    public class TrafficMonitoringPlug : IPlug, IOnChannelStarted, IOnChannelDisconnect, IAfterMessageReaded, IAfterMessageWrited {
        private Service _service;
        private readonly Dictionary<long, Box> _boxes = new();

        public void Init(IPluggable pluggable) {
            this._service = (Service)pluggable;
        }
        
        public void Dispose() {
            this._boxes.Clear();
        }

        void IOnChannelStarted.Handle(long channelId) {
            this.RemoveBox(channelId);
            var box = new Box();
            this._boxes.Add(channelId, box);
        }

        void IOnChannelDisconnect.Handle(long channelId) {
            this.RemoveBox(channelId);
        }
        
        void IAfterMessageReaded.Handle(long channel, ref Memory<byte> data) {
            if (this._boxes.TryGetValue(channel, out var box)) {
                box.totalBytesRecv += data.Length;
            }
        }

        void IAfterMessageWrited.Handle(long channel, ref Memory<byte> data) {
            if (this._boxes.TryGetValue(channel, out var box)) {
                box.totalBytesSend += data.Length;
            }
        }

        public (int tbr, int tbs) GetTraffic(long channelId) {
            if (this._boxes.TryGetValue(channelId, out var box))
                return (box.totalBytesRecv, box.totalBytesSend);

            return default;
        }

        private void RemoveBox(long channelId) {
            this._boxes.Remove(channelId);
        }

        private class Box {
            public int totalBytesRecv;
            public int totalBytesSend;

            public void Dispose() {
                this.totalBytesRecv = 0;
                this.totalBytesSend = 0;
            }
        }
    }
}