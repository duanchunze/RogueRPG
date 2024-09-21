using System;
using System.Runtime.InteropServices;
using UnityEngine.Serialization;

namespace Hsenl {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Int2 {
        public int x;
        public int y;

        public int this[int index] {
            get {
                return index switch {
                    0 => this.x,
                    1 => this.y,
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
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        public Int2(int x, int y) {
            this.x = x;
            this.y = y;
        }
    }
}