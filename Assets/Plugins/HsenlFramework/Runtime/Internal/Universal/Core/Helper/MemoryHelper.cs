using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Hsenl {
    public static class MemoryHelper {
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi)]
        public static extern void MemoryCopy(IntPtr dest, IntPtr src, uint count);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemoryCopy(byte[] dest, byte[] src, UIntPtr count);

        // public static T ObjectCopyOfMemory<T>(in T? value) {
        //     if (!RuntimeHelpers.IsReferenceOrContainsReferences<T>()) {
        //         var array = new byte[Unsafe.SizeOf<T>()];
        //         Unsafe.WriteUnaligned(ref GetArrayDataReference(array), value);
        //     }
        // }
        //
        // public static ref T GetArrayDataReference<T>(T[] array) {
        //     return ref MemoryMarshal.GetReference(array.AsSpan());
        // }
    }
}