using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace Hsenl.Network {
    // 重写是为了把_origin暴露出来, 因为需要用它给PackageBuffer做分层
    // 代码是直接拷贝的微软的
    public class HMemoryStream : Stream {
        private const int MemStreamMaxLength = int.MaxValue;

        private readonly ArrayPool<byte> _arrayPool;

        private byte[] _buffer;
        private int _origin;
        private int _position;
        private int _length;
        private int _capacity;

        private readonly bool _expandable;
        private readonly bool _writable;
        private readonly bool _exposable;
        private readonly bool _isOpen;

        public override bool CanRead => this._isOpen;
        public override bool CanSeek => this._isOpen;
        public override bool CanWrite => this._writable;

        public virtual int Origin {
            get => this._origin;
            set {
                if (value > this._position)
                    throw new ArgumentOutOfRangeException($"Origin cant not set more than position! '{value}'/'{this._position}'");
                this._origin = value;
            }
        }

        public override long Position {
            get {
                if (!this._isOpen)
                    throw new Exception("MemoryStream is not open!");

                return this._position - this._origin;
            }
            set {
                ThrowIfNegative(value);
                this.ThrowIfNotOpen();

                if (value > MemStreamMaxLength - this._origin)
                    throw new ArgumentOutOfRangeException(value.ToString());
                this._position = this._origin + (int)value;
            }
        }

        public override long Length {
            get {
                if (!this._isOpen)
                    throw new Exception("MemoryStream is not open!");
                return this._length - this._origin;
            }
        }

        /// <summary>
        /// 以origin为基准.
        /// 只做两件事, 如果array不足, 则补充, 并设置capacity, 除此之外, 不会设置其他值.
        /// 毕竟position永远不会超过capacity
        /// </summary>
        public virtual int Capacity {
            get {
                this.ThrowIfNotOpen();
                return this._capacity - this._origin;
            }
            set {
                if (value < this.Length)
                    throw new ArgumentOutOfRangeException(nameof(value), "ArgumentOutOfRange_SmallCapacity");

                this.ThrowIfNotOpen();

                if (!this._expandable && (value != this.Capacity))
                    throw new NotSupportedException("NotSupported_MemStreamNotExpandable");

                // MemoryStream has this invariant: _origin > 0 => !expandable (see ctors)
                if (this._expandable && value != this._capacity) {
                    if (value > 0) {
                        byte[] newBuffer = this._arrayPool?.Rent(value) ?? new byte[value];
                        if (this._arrayPool != null) {
                            Array.Clear(newBuffer, 0, newBuffer.Length);
                        }

                        if (this._length > 0) {
                            Buffer.BlockCopy(this._buffer, 0, newBuffer, 0, this._length);
                        }

                        this._arrayPool?.Return(this._buffer);
                        this._buffer = newBuffer;
                    }
                    else {
                        this._arrayPool?.Return(this._buffer);
                        this._buffer = Array.Empty<byte>();
                    }

                    this._capacity = value;
                }
            }
        }

        public HMemoryStream()
            : this(0) { }

        public HMemoryStream(int capacity, bool usePool = false) {
            if (capacity < 0)
                throw new ArgumentOutOfRangeException(capacity.ToString());

            if (usePool)
                this._arrayPool = ArrayPool<byte>.Shared;

            this._buffer = capacity != 0 ? new byte[capacity] : Array.Empty<byte>();
            this._capacity = capacity;
            this._expandable = true;
            this._writable = true;
            this._exposable = true;
            this._isOpen = true;
        }

        public override void Flush() { }

        public virtual byte[] GetBuffer() {
            if (!this._exposable)
                throw new UnauthorizedAccessException("UnauthorizedAccess_MemStreamBuffer");
            return this._buffer;
        }

        public virtual bool TryGetBuffer(out ArraySegment<byte> buffer) {
            if (!this._exposable) {
                buffer = default;
                return false;
            }

            buffer = new ArraySegment<byte>(this._buffer, offset: this._origin, count: this._length - this._origin);
            return true;
        }

        public override int Read(byte[] buffer, int offset, int count) {
            ValidateBufferArguments(buffer, offset, count);
            this.ThrowIfNotOpen();

            int n = this._length - this._position;
            if (n > count)
                n = count;
            if (n <= 0)
                return 0;

            Debug.Assert(this._position + n >= 0, "_position + n >= 0"); // len is less than 2^31 -1.

            if (n <= 8) {
                int byteCount = n;
                while (--byteCount >= 0)
                    buffer[offset + byteCount] = this._buffer[this._position + byteCount];
            }
            else
                Buffer.BlockCopy(this._buffer, this._position, buffer, offset, n);

            this._position += n;

            return n;
        }

        public override int Read(Span<byte> buffer) {
            if (this.GetType() != typeof(MemoryStream)) {
                // MemoryStream is not sealed, and a derived type may have overridden Read(byte[], int, int) prior
                // to this Read(Span<byte>) overload being introduced.  In that case, this Read(Span<byte>) overload
                // should use the behavior of Read(byte[],int,int) overload.
                return base.Read(buffer);
            }

            this.ThrowIfNotOpen();

            int n = Math.Min(this._length - this._position, buffer.Length);
            if (n <= 0)
                return 0;

            new Span<byte>(this._buffer, this._position, n).CopyTo(buffer);

            this._position += n;
            return n;
        }

        public override int ReadByte() {
            this.ThrowIfNotOpen();

            if (this._position >= this._length)
                return -1;

            return this._buffer[this._position++];
        }

        public override long Seek(long offset, SeekOrigin loc) {
            this.ThrowIfNotOpen();

            return this.SeekCore(offset, loc switch {
                SeekOrigin.Begin => this._origin,
                SeekOrigin.Current => this._position,
                SeekOrigin.End => this._length,
                _ => throw new ArgumentException()
            });
        }

        private long SeekCore(long offset, int loc) {
            if (offset > MemStreamMaxLength - loc)
                throw new ArgumentOutOfRangeException(nameof(offset), "ArgumentOutOfRange_StreamLength");
            int tempPosition = unchecked(loc + (int)offset);
            if (unchecked(loc + offset) < this._origin || tempPosition < this._origin)
                throw new IOException("IO_SeekBeforeBegin");
            this._position = tempPosition;

            Debug.Assert(this._position >= this._origin, "_position >= _origin");
            return this._position - this._origin;
        }

        // 以origin为基准.
        // 设置长度, 如果设置的长度比当前position小, 则不操作, 如果比当前position大, 则拓展array, 并把position设置为当前长度
        public override void SetLength(long value) {
            if (value < 0 || value > int.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(value), "ArgumentOutOfRange_StreamLength");

            this.ThrowIfNotWriteable();

            Debug.Assert(MemStreamMaxLength == int.MaxValue);
            if (value > (int.MaxValue - this._origin))
                throw new ArgumentOutOfRangeException(nameof(value), "ArgumentOutOfRange_StreamLength");

            int newLength = this._origin + (int)value;
            bool allocatedNewArray = this.EnsureCapacity(newLength);
            if (!allocatedNewArray && newLength > this._length)
                Array.Clear(this._buffer, this._length, newLength - this._length);
            this._length = newLength;
            if (this._position > newLength) this._position = newLength;
        }

        public override void Write(byte[] buffer, int offset, int count) {
            ValidateBufferArguments(buffer, offset, count);
            this.ThrowIfNotOpen();
            this.ThrowIfNotWriteable();

            int i = this._position + count;
            if (i < 0)
                throw new IOException("IO_StreamTooLong");

            if (i > this._length) {
                bool mustZero = this._position > this._length;
                if (i > this._capacity) {
                    bool allocatedNewArray = this.EnsureCapacity(i);
                    if (allocatedNewArray) {
                        mustZero = false;
                    }
                }

                if (mustZero) {
                    Array.Clear(this._buffer, this._length, i - this._length);
                }

                this._length = i;
            }

            if ((count <= 8) && (buffer != this._buffer)) {
                int byteCount = count;
                while (--byteCount >= 0) {
                    this._buffer[this._position + byteCount] = buffer[offset + byteCount];
                }
            }
            else {
                Buffer.BlockCopy(buffer, offset, this._buffer, this._position, count);
            }

            this._position = i;
        }

        public override void Write(ReadOnlySpan<byte> buffer) {
            if (this.GetType() != typeof(HMemoryStream)) {
                // MemoryStream is not sealed, and a derived type may have overridden Write(byte[], int, int) prior
                // to this Write(Span<byte>) overload being introduced.  In that case, this Write(Span<byte>) overload
                // should use the behavior of Write(byte[],int,int) overload.
                base.Write(buffer);
                return;
            }

            this.ThrowIfNotOpen();
            this.ThrowIfNotWriteable();

            int i = this._position + buffer.Length;
            if (i < 0)
                throw new IOException("IO_StreamTooLong");

            if (i > this._length) {
                bool mustZero = this._position > this._length;
                if (i > this._capacity) {
                    bool allocatedNewArray = this.EnsureCapacity(i);
                    if (allocatedNewArray) {
                        mustZero = false;
                    }
                }

                if (mustZero) {
                    Array.Clear(this._buffer, this._length, i - this._length);
                }

                this._length = i;
            }

            buffer.CopyTo(new Span<byte>(this._buffer, this._position, buffer.Length));
            this._position = i;
        }

        public override void WriteByte(byte value) {
            this.ThrowIfNotOpen();
            this.ThrowIfNotWriteable();

            if (this._position >= this._length) {
                int newLength = this._position + 1;
                bool mustZero = this._position > this._length;
                if (newLength >= this._capacity) {
                    bool allocatedNewArray = this.EnsureCapacity(newLength);
                    if (allocatedNewArray) {
                        mustZero = false;
                    }
                }

                if (mustZero) {
                    Array.Clear(this._buffer, this._length, this._position - this._length);
                }

                this._length = newLength;
            }

            this._buffer[this._position++] = value;
        }

        public virtual void WriteTo(Stream stream) {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            this.ThrowIfNotOpen();

            stream.Write(this._buffer, this._origin, this._length - this._origin);
        }

        public virtual byte[] ToArray() {
            int count = this._length - this._origin;
            if (count == 0)
                return Array.Empty<byte>();
            byte[] copy = new byte[count];
            this._buffer.AsSpan(this._origin, count).CopyTo(copy);
            return copy;
        }

        private bool EnsureCapacity(int value) {
            if (value < 0)
                throw new IOException("IO_StreamTooLong");

            if (value > this._capacity) {
                int newCapacity = Math.Max(value, 256);

                if (newCapacity < this._capacity * 2) {
                    newCapacity = this._capacity * 2;
                }

                if ((uint)(this._capacity * 2) > 2147483591U) {
                    newCapacity = Math.Max(value, 2147483591);
                }

                this.Capacity = newCapacity;
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfNotOpen() {
            if (!this._isOpen)
                throw new Exception("MemoryStream is not open!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ThrowIfNotWriteable() {
            if (!this._isOpen)
                throw new Exception("MemoryStream is not writeable!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ThrowIfNegative(long value) {
            if (value < 0)
                throw new ArgumentOutOfRangeException(value.ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ValidateBufferArguments(byte[] buffer, int offset, int count) {
            if (buffer == null)
                throw new ArgumentNullException();
            if (offset < 0)
                throw new ArgumentOutOfRangeException();
            if ((long)(uint)count <= (long)(buffer.Length - offset))
                return;
            throw new ArgumentOutOfRangeException();
        }
    }
}