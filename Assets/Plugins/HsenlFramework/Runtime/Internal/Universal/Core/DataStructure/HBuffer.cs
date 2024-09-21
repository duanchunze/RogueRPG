using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;

// ReSharper disable UnusedParameter.Local

namespace Hsenl {
    public class HBuffer : HMemoryStream, IBufferWriter<byte> {
        public new int Position {
            get => (int)base.Position;
            set => base.Position = value;
        }

        public new int Length => (int)base.Length;

        public HBuffer(bool usePool = false) : base(0, usePool) { }

        public HBuffer(int capacity, bool usePool = false) : base(capacity, usePool) { }

        // 根据当前position, 把position推进到某个位置, 一般是你写了多少数据, 就推进多少数据, 也可以先推进, 后面再写, 能自动扩容.
        // 如果是继续写入数据, 则调用该api, 因为他会在原有position的基础上, 进行内存拓展, 他不会复用之前的内存, 而是拓展出新的内存供你使用.
        public void Advance(int count) {
            var length = this.Position + count;
            if (length > this.Length)
                this.SetLength(length);
            this.Position = length;
        }

        // 以当前position为起点, 返回sizeHint长度的内存.
        // 能自动扩容, 但不会设置position, 所以如果不想得到重复的内存的话, 需要搭配Advance方法一起使用.
        // 和GetSpan效果一样.
        public Memory<byte> GetMemory(int sizeHint = 0) {
            var length = this.Length;
            var position = this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
            }

            var memory = this.AsMemory(position, sizeHint);
            return memory;
        }

        public Span<byte> GetSpan(int sizeHint = 0) {
            var length = this.Length;
            var position = this.Position;
            if (length - position < sizeHint) {
                this.SetLength(position + sizeHint);
            }

            var span = this.AsSpan(position, sizeHint);
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