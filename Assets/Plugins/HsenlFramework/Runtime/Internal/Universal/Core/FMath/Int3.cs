using System;
using System.Runtime.InteropServices;

namespace Hsenl {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Int3 {
        public int x;
        public int y;
        public int z;

        public int this[int index] {
            get {
                return index switch {
                    0 => this.x,
                    1 => this.y,
                    2 => this.z,
                    _ => throw new IndexOutOfRangeException("vector idx invalid" + index)
                };
            }
            set {
                switch (index) {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        public Int3(int x, int y, int z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}