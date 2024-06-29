using System;
using System.IO;

namespace Hsenl.Network {
    public class TcpPacketSender : IPacketSendProvider, IMessageWriter {
        private const int BodySizeBits = 4;
        private const int TotalHeadBits = 4;
        private const int MaxBufferSize = 1024 * 1024; // 当一个buffer的缓存被括的超过该值, 则进行警报, 看看发生什么事了

        private PackageBuffer _bufferSending = new(); // 两个send缓存区, 一个用来发送, 一个用来写入, 谁没在发送中, 谁当写入
        private PackageBuffer _bufferWriting = new();

        public Action<Memory<byte>> OnMessageWrited { get; set; } // 当写入了包(包括包装包)

        private readonly object locker = new();

        public virtual void Init() { }

        public void Init<T>(T t) { }

        // 写入后, 可以不用着急马上Send, 可以等到帧末调用一次Send, 这样既不会等待一帧的延迟, 同时也可以避免频繁的send, 占用通道.
        // 支持多线程写入
        public void Write(byte[] data, int offset, int count) {
            this.Write(data.AsSpan(offset, count));
        }

        public void Write(Span<byte> data) {
            if (data.Length == 0)
                return;
            
            lock (this.locker) {
                if (this._bufferWriting.Length > MaxBufferSize) {
                    // 如果缓存区写入过大, 可能因为意外原因, 导致积压的数据迟迟没有发出去, 这时先暂停写入
                    Log.Error("Writer buffer too large, temporarily stop writing, please send in time!");
                    return;
                }

                var len = data.Length;
                this._bufferWriting.GetSpan(BodySizeBits).WriteTo(len); // 写入头部大小
                this._bufferWriting.Advance(TotalHeadBits);
                this._bufferWriting.Write(data); // 写入消息体
        
                try {
                    this.OnMessageWrited?.Invoke(this._bufferWriting.AsMemory(BodySizeBits, len));
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public byte[] GetSendBuffer(int len, out int offset, out int count) {
            throw new NotImplementedException();
        }

        public virtual Memory<byte> GetSendBuffer(int length) {
            var len = this._bufferSending.Position;

            // len == 0, 说明没有数据要发了
            if (len == 0) {
                lock (this.locker) {
                    if (this._bufferWriting.Position != 0) {
                        ObjectHelper.Swap(ref this._bufferSending, ref this._bufferWriting);
                        len = this._bufferSending.Position;
                    }
                    else {
                        return this._bufferSending.AsMemory(0, 0);
                    }
                }
            }

            if (len > length) {
                len = length;
            }

            var buffer = this._bufferSending.AsMemory(0, len);
            this._bufferSending.Origin += len;

            if (this._bufferSending.Position <= 0) {
                // 所有数据都发送完了, 重置buffer
                this._bufferSending.Origin = 0;
                this._bufferSending.Seek(0, SeekOrigin.Begin);
            }

            return buffer;
        }

        public virtual void Dispose() {
            this.OnMessageWrited = null;
            this._bufferSending?.Reset();
            this._bufferWriting?.Reset();
        }
    }
}