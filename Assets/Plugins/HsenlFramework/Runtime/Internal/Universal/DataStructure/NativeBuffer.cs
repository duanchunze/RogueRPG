using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl {
    // NativeBuffer<int> buf = new(new[] { 1, 2, 3, 4, 5 });
    // Console.WriteLine(buf[3]); // 4
    // buf[2] = 9;
    // Console.WriteLine(buf[2]); // 9
    // // ...
    // buf.Dispose();
    // 使用的是直接分配内存的方式来实现的array, 由于语言版本的问题, 对于foreach中还不支持ref
    public sealed class NativeBuffer<T> : IDisposable where T : unmanaged {
        private unsafe T* _pointer;
        public nint Length { get; }

        public NativeBuffer(nint length) {
            this.Length = length;
            unsafe {
                this._pointer = (T*)Marshal.AllocHGlobal(length * sizeof(T));
            }
        }

        public NativeBuffer(Span<T> span) : this(span.Length) {
            unsafe {
                fixed (T* ptr = span) {
                    Buffer.MemoryCopy(ptr, this._pointer, sizeof(T) * span.Length, sizeof(T) * span.Length);
                }
            }
        }

        [DoesNotReturn]
        private static ref T ThrowOutOfRange() => throw new IndexOutOfRangeException();

        public ref T this[nint index] {
            get {
                unsafe {
                    return ref (index >= this.Length ? ref ThrowOutOfRange() : ref (*(this._pointer + index)));
                }
            }
        }

        public void Dispose() {
            unsafe {
                // 判断内存是否有效
                if (this._pointer != (T*)0) {
                    Marshal.FreeHGlobal((IntPtr)this._pointer);
                    this._pointer = (T*)0;
                }
            }
        }

        // 双保险, 即使没有调用 Dispose 也可以在 GC 回收时释放资源
        ~NativeBuffer() {
            this.Dispose();
        }

        public ref struct NativeBufferEnumerator {
            private readonly unsafe T* pointer;
            private readonly nint length;
            private T current;
            private nint index;

            public T Current {
                get {
                    unsafe {
                        // 确保指向的内存仍然有效
                        if (this.pointer == (T*)0) {
                            return Unsafe.NullRef<T>();
                        }
                        else return this.current;
                    }
                }
            }

            public unsafe NativeBufferEnumerator(ref T* pointer, nint length) {
                this.pointer = pointer;
                this.length = length;
                this.index = 0;
                this.current = Unsafe.NullRef<T>();
            }

            public bool MoveNext() {
                unsafe {
                    // 确保没有越界并且指向的内存仍然有效
                    if (this.index >= this.length || this.pointer == (T*)0) {
                        return false;
                    }

                    if (Unsafe.IsNullRef(ref this.current))
                        this.current = *this.pointer;
                    else
                        this.current = Unsafe.Add(ref this.current, 1);
                }

                this.index++;
                return true;
            }
        }
    }
}