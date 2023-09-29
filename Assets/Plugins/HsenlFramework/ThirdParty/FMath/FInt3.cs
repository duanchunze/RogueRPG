using System;
using System.Runtime.InteropServices;

namespace FixedMath {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct FInt3 {
        public int a, b, c;

        public int this[int index] {
            get {
                return index switch {
                    0 => this.a,
                    1 => this.b,
                    2 => this.c,
                    _ => throw new IndexOutOfRangeException("vector idx invalid" + index)
                };
            }
            set {
                switch (index) {
                    case 0:
                        this.a = value;
                        break;
                    case 1:
                        this.b = value;
                        break;
                    case 2:
                        this.c = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        public FInt3(int a, int b, int c) {
            this.a = a;
            this.b = b;
            this.c = c;
        }
    }
}