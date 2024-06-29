using System;

namespace Hsenl.Network {
    // 断线重连
    public class ReconnectPlug : IPlug, IOnChannelDisconnect {
        private int _reconnectInterval;
        private int _tryNum;

        private TcpClient _tcpClient;
        private KcpClient _kcpClient;

        public Action OnReconnectFail;

        public ReconnectPlug(int reconnectIntervalSeconds = 5, int tryNum = 5) {
            this._reconnectInterval = reconnectIntervalSeconds;
            this._tryNum = tryNum;
        }

        public void Init(IPluggable pluggable) {
            switch (pluggable) {
                case TcpClient tcpClient:
                    this._tcpClient = tcpClient;
                    break;
                case KcpClient kcpClient:
                    this._kcpClient = kcpClient;
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Reconnect only using on tcp client! '{pluggable.GetType()}'");
            }
        }

        public void Dispose() {
            this._tcpClient = null;
            this._kcpClient = null;
            this._reconnectInterval = 0;
            this._tryNum = 0;
        }

        public void Handle(long channelId) {
            try {
                this.Start();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void Start() {
            if (this._tcpClient?.IsDisposed ?? false)
                return;

            if (this._kcpClient?.IsDisposed ?? false)
                return;

            this.ContinueReconnect().Tail();
        }

        private async HVoid ContinueReconnect() {
            var tryCount = this._tryNum;
            if (this._tcpClient != null) {
                while (tryCount-- > 0) {
                    // 否则等待后, 继续尝试重连
                    await Timer.WaitTime(this._reconnectInterval * 1000);

                    if (this._tcpClient == null)
                        return;

                    // 如果client已经被销毁了, 则不会重连了
                    if (this._tcpClient.IsDisposed)
                        return;

                    // 如果已经连上了, 则跳出
                    if (this._tcpClient.IsConnected)
                        return;

                    // 进行重连
                    await this._tcpClient.ConnectAsync();

                    if (this._tcpClient == null)
                        return;

                    // 如果已经连上了, 则跳出
                    if (this._tcpClient.IsConnected)
                        return;
                }
            }
            else {
                while (tryCount-- > 0) {
                    // 否则等待后, 继续尝试重连
                    await Timer.WaitTime(this._reconnectInterval * 1000);

                    if (this._kcpClient == null)
                        return;

                    // 如果client已经被销毁了, 则不会重连了
                    if (this._kcpClient.IsDisposed)
                        return;

                    // 如果已经连上了, 则跳出
                    if (this._kcpClient.IsConnected)
                        return;

                    // 进行重连
                    await this._kcpClient.ConnectAsync();

                    if (this._kcpClient == null)
                        return;

                    // 如果已经连上了, 则跳出
                    if (this._kcpClient.IsConnected)
                        return;
                }
            }


            // 次数用尽依然没有重连成功
            this.OnReconnectFail?.Invoke();
        }
    }
}