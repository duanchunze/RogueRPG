using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl {
    public unsafe readonly struct UnmanagedArray<T> where T : unmanaged {
        private readonly T* ptr;
        public int Length { get; }

        public UnmanagedArray(int cap) {
            this.Length = cap;
            this.ptr = (T*)Marshal.AllocHGlobal(cap * sizeof(T));
            if (this.ptr == null) throw new Exception("alloc HGlobal fail");

            for (var i = 0; i < cap; i++) {
                this.ptr[i] = default;
            }
        }
        
        public T this[int index] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *(this.ptr + index);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => *(this.ptr + index) = value;
        }

        public void Dispose() {
            Marshal.FreeHGlobal((IntPtr)this.ptr);
        }
    }
}