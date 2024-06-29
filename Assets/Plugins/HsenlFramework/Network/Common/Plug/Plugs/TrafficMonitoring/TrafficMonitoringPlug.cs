using System;
using System.Collections.Generic;
using System.Net;

namespace Hsenl.Network {
    // 流量监控. 每个channel发送与接收了多少数据
    public class TrafficMonitoringPlug : IPlug, IOnChannelStarted, IOnChannelDisconnect {
        private Service _service;
        private readonly Dictionary<long, Box> _boxes = new();

        public void Init(IPluggable pluggable) {
            this._service = (Service)pluggable;
            this._service.OnRecvMessage += this.OnRecvMessage;
            this._service.OnSendMessage += this.OnSendMessage;
        }
        
        public void Dispose() {
            this._service.OnRecvMessage -= this.OnRecvMessage;
            this._service.OnSendMessage -= this.OnSendMessage;
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

        private void OnRecvMessage(long channelId, Memory<byte> message) {
            if (this._boxes.TryGetValue(channelId, out var box)) {
                box.totalBytesRecv += message.Length;
            }
        }

        private void OnSendMessage(long channelId, Memory<byte> message) {
            if (this._boxes.TryGetValue(channelId, out var box)) {
                box.totalBytesSend += message.Length;
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