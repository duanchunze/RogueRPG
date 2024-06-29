using System;

namespace Hsenl.Network {
    // 一个专门的通道, 用于处理数据, 包括: 提供接收数据缓存, 提供要发送的数据缓存, 阅读接收的数据, 写入消息.
    // 相当于是数据的中转站, 把消息转成数据, 并交给socket发送, 同时, 又把socket接受的数据翻译成可用的具体消息.
    // 
    // 例如: 与IOCPSocket进行配合, 前者只需要关注数据收发部分, 具体的拆粘包, 等等进一步的处理交给channel.
    // 通道不直接与socket和service交互
    public abstract class Channel {
        public long ChannelId { get; private set; }
        internal object UserToken { get; set; }
        internal Action<long, Memory<byte>> OnRecvMessage { get; set; }
        internal Action<long, Memory<byte>> OnSendMessage { get; set; }
        internal Action<long, int> OnError { get; set; }

        public bool IsDisposed => this.ChannelId == 0;

        internal virtual void Init(long channelId) {
            if (!this.IsDisposed)
                throw new Exception("Channel is not disposed, cant Init");

            this.ChannelId = channelId;
        }

        internal abstract Memory<byte> GetRecvBuffer(int len);
        internal abstract Memory<byte> GetSendBuffer(int len);
        internal abstract byte[] GetRecvBuffer(int len, out int offset, out int count);
        internal abstract byte[] GetSendBuffer(int len, out int offset, out int count);
        internal abstract void Read(Memory<byte> data); // 读入的数据, 处理后从OnRecvMessage回调出来
        internal abstract void Write(byte[] data, int offset, int count); // 写入数据, 处理后从OnSendMessage回调出来
        internal abstract void Write(Span<byte> data);

        protected void Error(int errorCode) {
            try {
                this.OnError?.Invoke(this.ChannelId, errorCode);
            }
            catch (Exception exception) {
                Log.Error(exception);
            }
        }

        internal virtual void Dispose() {
            this.ChannelId = 0;
            this.UserToken = null;
            this.OnRecvMessage = null;
            this.OnSendMessage = null;
            this.OnError = null;
        }

        protected void CheckDisposedException() {
            if (this.IsDisposed)
                throw new Exception("Channel is disposed!");
        }
    }
}