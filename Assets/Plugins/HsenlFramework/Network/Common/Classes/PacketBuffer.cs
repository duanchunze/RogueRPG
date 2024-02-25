using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedParameter.Local

namespace Hsenl.Network {
    /*
     * EventArgs消息的收发, 序列化与反序列化, 都操作这一个buffer, 操作的始终是一块内存, 减少了2次拷贝次数.
     * 对于EventArgs来说, 设置其SetBuffer为Memory<byte>, 能省掉一次从内核缓冲区到用户缓存区的拷贝操作.
     * 直接在buffer的内存上序列化, 可以避免一次向EventArgs缓冲区的拷贝操作.
     */
    public class PacketBuffer : MemoryStream, IBufferWriter<byte> {
        private int _origin;
        private int _writePoint;
        
        public int Origin {
            get => this._origin;
            set {
                this._origin = value;
                var position = (int)this.Position;
                if (this._origin > position) {
                    this._origin = position;
                }
            }
        }

        public bool HasData => this._origin != this.Position;

        public PacketBuffer() { }

        public PacketBuffer(int capacity) : base(capacity) { }

        public PacketBuffer(byte[] buffer, int index, int count) => throw new NotImplementedException(); // 禁用父类的origin, 使用我们自己的
        public PacketBuffer(byte[] buffer, int index, int count, bool writable) => throw new NotImplementedException();
        public PacketBuffer(byte[] buffer, int index, int count, bool writable, bool publiclyVisible) => throw new NotImplementedException();
        
        public void RecordWritePoint() {
            this._writePoint = (int)this.Position;
        }

        public int EndWriteRecord() {
            return (int)this.Position - this._writePoint;
        }

        // 把position推进到某个位置, 一般是刚写入的数据的末尾, 把length设置为从start到position的长度, 能自动扩容
        // memory pack在使用中, 可能会通过下面的GetSpan把length扩的很大, 但实际写入的却很少, 例如扩1000, 用100, 这个100会返回到这里
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Advance(int count) {
            var length = this.Position + count;
            if (length > this.Length)
                this.SetLength(length);
            this.Position = length;
        }

        // 得到一块可以写入的内存块, 能自动扩容, 扩容后会更新length, 和GetSpan效果一样
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> GetMemory(int sizeHint = 0) {
            var length = (int)this.Length;
            var position = (int)this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
                length = (int)this.Length;
            }

            var memory = this.AsMemory(position, length - position);
            return memory;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> GetSpan(int sizeHint = 0) {
            var length = (int)this.Length;
            var position = (int)this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
                length = (int)this.Length;
            }

            var span = this.AsSpan(position, length - position);
            return span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory() => this.GetBuffer().AsMemory(this._origin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start) => this.GetBuffer().AsMemory(this._origin + start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start, int length) => this.GetBuffer().AsMemory(this._origin + start, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => this.GetBuffer().AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start) => this.GetBuffer().AsSpan(this._origin + start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start, int length) => this.GetBuffer().AsSpan(this._origin + start, length);

        public void Reset() {
            this._origin = 0;
            this._writePoint = 0;
            this.Position = 0;
            this.SetLength(0);
        }
    }
}