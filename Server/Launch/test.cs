using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Launch;

public class test {
    public int a;
    
    public static unsafe void dfdfd() {
        // Buffer.BlockCopy();
        // Buffer.MemoryCopy();
        // Unsafe.CopyBlock();
        // Marshal.Copy();

        string[] strSrc = new string[] { "Adc", "Bbc" };
        string[] strDst = new string[2];

        Struct[] structSource = new Struct[] { new Struct() { a = 1 }, new Struct() { a = 2 } };
        Struct[] structDst = new Struct[2];

        int[] intSource = new int[] { 1, 2 };
        int[] intDst = new int[2];

        // 错误的尝试：不能直接用Buffer.BlockCopy()复制字符串数组
        // Buffer.BlockCopy(sourceArray, 0, destinationArra, 0, sourceArray.Length * sizeof(Struct));
        
        fixed (string* ptrSrc = strSrc) {
            fixed (string* ptrDst = strDst) {
                // Buffer.MemoryCopy(ptrSrc, ptrDst, strSrc.Length * 8, strDst.Length * 8);
                Unsafe.CopyBlock(ptrDst, ptrSrc, (uint)strSrc.Length * 8);
            }
        }

        Console.WriteLine(strDst[0]);
        Console.WriteLine(strDst[1]);


        fixed (int* ptrSrc = intSource) {
            fixed (int* ptrDst = intDst) {
                // Buffer.MemoryCopy(ptrSrc, ptrDst, 8, 8);
                Unsafe.CopyBlock(ptrDst, ptrSrc, 8);
            }
        }

        Console.WriteLine(intDst[0]);
        Console.WriteLine(intDst[1]);


        fixed (Struct* ptrSrc = structSource) {
            fixed (Struct* ptrDst = structDst) {
                Buffer.MemoryCopy(ptrSrc, ptrDst, 8, 8);
            }
        }

        Console.WriteLine(structDst[0].a);
        Console.WriteLine(structDst[1].a);
        
        MemoryPack.MemoryPackSerializer.Serialize()
    }

    public struct Struct {
        public int a;
    }
}