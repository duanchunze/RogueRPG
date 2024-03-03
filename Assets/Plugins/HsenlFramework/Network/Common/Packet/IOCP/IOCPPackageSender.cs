using System;
using System.IO;
using System.Threading;

namespace Hsenl.Network {
    public class IOCPPackageSender : IPackageSendBufferProvider, IPackageWriter {
        protected virtual int BodySizeBits => 4;
        protected virtual int OpcodeBits => 2;
        protected virtual int TotalHeadBits => 6;

        private PackageBuffer _sendBufferSending = new(); // 两个send缓存区, 一个用来发送, 一个用来写入, 谁没在发送中, 谁当写入
        private PackageBuffer _sendBufferWriting = new();
        
        public Action<ushort, Memory<byte>> OnMessageWrited { get; set; } // 当写入了包(包括包装包)

        private readonly object locker = new();

        public virtual void Init() { }

        // 写入后, 可以不用着急马上Send, 可以等到帧末调用一次Send, 这样既不会等待一帧的延迟, 同时也可以避免频繁的send, 占用通道.
        // 支持多线程写入
        public virtual void Write(Func<PackageBuffer, ushort> write) {
            lock (this.locker) {
                var position = this._sendBufferWriting.Position;
                this._sendBufferWriting.Advance(this.TotalHeadBits);
                var anchor = this._sendBufferWriting.Position;
                var opcode = write.Invoke(this._sendBufferWriting);
                var len = this._sendBufferWriting.Position - anchor;
                if (len <= 0)
                    throw new Exception($"write data can not <= 0 '{len}'");
                this._sendBufferWriting.AsSpan(position, this.BodySizeBits).WriteTo(len);
                this._sendBufferWriting.AsSpan(position + this.BodySizeBits, this.OpcodeBits).WriteTo(opcode);

                try {
                    this.OnMessageWrited?.Invoke(opcode, this._sendBufferWriting.AsMemory(anchor, len));
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public virtual Memory<byte> GetSendBuffer(int min, int max) {
            lock (this.locker) {
                var len = this._sendBufferSending.Position;

                if (len == 0)
                    return this._sendBufferSending.AsMemory(0, 0);

                if (len > max) {
                    len = max;
                }

                var buffer = this._sendBufferSending.AsMemory(0, len);
                this._sendBufferSending.Origin += len;

                if (this._sendBufferSending.Position <= 0) {
                    // 所有数据都发送完了, 重置buffer
                    this._sendBufferSending.Origin = 0;
                    this._sendBufferSending.Seek(0, SeekOrigin.Begin);
                }

                return buffer;
            }
        }

        public virtual bool SendNext() {
            lock (this.locker) {
                if (this._sendBufferSending.Position == 0) {
                    if (this._sendBufferWriting.Position == 0) {
                        return false;
                    }

                    ObjectHelper.Swap(ref this._sendBufferSending, ref this._sendBufferWriting);
                }

                return true;
            }
        }

        public virtual void Reset() {
            this.OnMessageWrited = null;
            this._sendBufferSending?.Reset();
            this._sendBufferWriting?.Reset();
        }

        public virtual void Dispose() {
            this.Reset();
            this._sendBufferSending = null;
            this._sendBufferWriting = null;
        }
    }
}