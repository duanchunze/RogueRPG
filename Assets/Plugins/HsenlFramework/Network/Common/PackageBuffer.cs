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
    public class PackageBuffer : HMemoryStream, IBufferWriter<byte> {
        public new int Position {
            get => (int)base.Position;
            set => base.Position = value;
        }

        public new int Length {
            get => (int)base.Length;
            set => base.Position = value;
        }

        public PackageBuffer() { }

        public PackageBuffer(int capacity) : base(capacity) { }

        // 把position推进到某个位置, 一般是你写了多少数据, 就推进多少数据, 也可以先推进, 后面再写, 能自动扩容
        public void Advance(int count) {
            var length = this.Position + count;
            if (length > this.Length)
                this.SetLength(length);
            this.Position = length;
        }

        // 得到一块可以写入的内存块, 能自动扩容, 扩容后会更新length, 和GetSpan效果一样
        public Memory<byte> GetMemory(int sizeHint = 0) {
            var length = this.Length;
            var position = this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
                length = this.Length;
            }

            var memory = this.AsMemory(position, length - position);
            return memory;
        }

        public Span<byte> GetSpan(int sizeHint = 0) {
            var length = this.Length;
            var position = this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
                length = this.Length;
            }

            var span = this.AsSpan(position, length - position);
            return span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory() => this.GetBuffer().AsMemory(this.Origin);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start) => this.GetBuffer().AsMemory(this.Origin + start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Memory<byte> AsMemory(int start, int length) => this.GetBuffer().AsMemory(this.Origin + start, length);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan() => this.GetBuffer().AsSpan();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start) => this.GetBuffer().AsSpan(this.Origin + start);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan(int start, int length) => this.GetBuffer().AsSpan(this.Origin + start, length);

        public void Reset() {
            this.Origin = 0;
            this.SetLength(0);
        }
    }
}