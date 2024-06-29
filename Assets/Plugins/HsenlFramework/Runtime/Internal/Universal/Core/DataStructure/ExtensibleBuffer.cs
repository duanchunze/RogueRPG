using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Hsenl {
    /// <summary>
    /// A buffer that can dynamically extend
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed unsafe class ExtensibleBuffer<T> where T : unmanaged {
        /// <summary>
        /// Default size of the buffer
        /// </summary>
        private const int DefaultBufferSize = 128;

        /// <summary>
        /// Data that stores everything
        /// </summary>
        public T* Data { get; private set; }

        /// <summary>
        /// Size of T
        /// </summary>
        private readonly byte sizeOfT;

        /// <summary>
        /// expand size for each block
        /// </summary>
        public readonly int ExpandSize;

        /// <summary>
        /// Total length of the buffer
        /// </summary>
        public int TotalLength { get; private set; }

        /// <summary>
        /// Init buffer
        /// </summary>
        public ExtensibleBuffer() : this(DefaultBufferSize) { }

        /// <summary>
        /// Init extensible buffer with a capacity
        /// </summary>
        /// <param name="size"></param>
        public ExtensibleBuffer([In] int size = DefaultBufferSize) {
            this.sizeOfT = (byte)sizeof(T);
            this.ExpandSize = size;
            this.Data = (T*)Marshal.AllocHGlobal(this.sizeOfT * this.ExpandSize);
            this.TotalLength = this.ExpandSize;
            GC.AddMemoryPressure(this.sizeOfT * this.ExpandSize);
        }

        /// <summary>
        /// Get element at index
        /// </summary>
        /// <param name="index"></param>
        public T this[in int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *(this.Data + index);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                this.EnsureCapacity(in index);
                *(this.Data + index) = value;
            }
        }

        /// <summary>
        /// Ensure index exists
        /// </summary>
        /// <param name="index"></param>
        private void EnsureCapacity(in int index) {
            if (index < this.TotalLength) return;
            GC.RemoveMemoryPressure(this.TotalLength * this.sizeOfT);
            while (index >= this.TotalLength) {
                this.TotalLength += this.ExpandSize;
            }

            this.Extend();
            GC.AddMemoryPressure(this.TotalLength * this.sizeOfT);
        }

        /// <summary>
        /// Extend buffer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Extend() {
            this.Data = (T*)Marshal.ReAllocHGlobal((IntPtr)this.Data, new IntPtr(this.TotalLength * this.sizeOfT));
        }

        /// <summary>
        /// Convert buffer data to an Array (will create a new array and copy values)
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public T[] ToArray([In] int startIndex, [In] int length) {
            T[] ret = new T[length];
            this.CopyTo(ref ret, startIndex, length);
            return ret;
        }

        /// <summary>
        /// convert an extensible to buffer from start index with provided length
        /// </summary>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public Span<T> AsSpan([In] int startIndex, [In] int length) {
            var l = startIndex + length;
            //size check
            this.EnsureCapacity(in l);
            return new Span<T>(this.Data + startIndex, length);
        }

        /// <summary>
        /// Convert to span
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        public static implicit operator Span<T>(ExtensibleBuffer<T> buffer) => buffer.AsSpan(0, buffer.TotalLength);

        /// <summary>
        /// Copy data to extensible buffer
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcIndex"></param>
        /// <param name="dstIndex"></param>
        /// <param name="length"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyFrom(T[] src, [In] int srcIndex, [In] int dstIndex, [In] int length) {
            fixed (T* ptr = src) {
                this.CopyFrom(ptr, srcIndex, dstIndex, length);
            }
        }

        /// <summary>
        /// Copy data to extensible buffer
        /// why unaligned? https://stackoverflow.com/a/72418388
        /// </summary>
        /// <param name="src"></param>
        /// <param name="srcIndex"></param>
        /// <param name="dstIndex"></param>
        /// <param name="length"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void CopyFrom([In] T* src, [In] int srcIndex, [In] int dstIndex, [In] int length) {
            var l = dstIndex + length;
            //size check
            this.EnsureCapacity(in l);
            //copy
            Unsafe.CopyBlock(this.Data + dstIndex, src + srcIndex, (uint)length);
        }

        /// <summary>
        /// Copy data from buffer to dst from dst[0]
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="srcIndex"></param>
        /// <param name="length"></param>
        /// <exception cref="OverflowException"></exception>
        public void CopyTo(ref T[] dst, [In] int srcIndex, [In] int length) {
            fixed (T* ptr = dst) {
                this.CopyTo(ptr, srcIndex, length);
            }
        }

        /// <summary>
        /// Copy data from buffer to dst from dst[0]
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="srcIndex"></param>
        /// <param name="length"></param>
        /// <exception cref="OverflowException"></exception>
        public void CopyTo([In] T* dst, [In] int srcIndex, [In] int length) {
            var l = srcIndex + length;
            //size check
            this.EnsureCapacity(in l);
            //copy
            Unsafe.CopyBlock(dst, this.Data + srcIndex, (uint)length);
        }

        /// <summary>
        /// Free allocated memories
        /// </summary>
        ~ExtensibleBuffer() {
            Marshal.FreeHGlobal((IntPtr)this.Data);
            GC.RemoveMemoryPressure(this.sizeOfT * this.TotalLength);
        }
    }
}