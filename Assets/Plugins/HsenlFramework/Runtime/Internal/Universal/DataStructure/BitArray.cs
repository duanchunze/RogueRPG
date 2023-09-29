using System;

namespace Hsenl {
    // 专门用于enum判存的位数组，效率比直接使用List<Enum>的方式快很多，且可以在ECS系统中使用
    // 缺点就是需要手动修改代码来实现最大容量的扩容，但也十分简单
    [Serializable]
    public struct BitArray {
        [Serializable]
        private struct BitElement {
            private ulong m_L0;
            // private ulong m_L1;
            // private ulong m_L2;
            // private ulong m_L3;

            public int Length => 1;

            public ulong this[int index] {
                get {
                    return index switch {
                        0 => this.m_L0,
                        // 1 => this.m_L1,
                        // 2 => this.m_L2,
                        // 3 => this.m_L3,
                        _ => throw new IndexOutOfRangeException($"BitElement.Get index out of max '{index}'")
                    };
                }
                set {
                    switch (index) {
                        case 0:
                            this.m_L0 = value;
                            break;
                        // case 1: this.m_L1 = value; break;
                        // case 2: this.m_L2 = value; break;
                        // case 3: this.m_L3 = value; break;
                        default:
                            throw new IndexOutOfRangeException($"BitElement.Set index out of max '{index}'");
                    }
                }
            }

            public bool IsNull =>
                this.m_L0 == 0;
            // + this.m_L1
            // + this.m_L2
            // + this.m_L3 == 0;

            public void Clear() {
                this.m_L0 = 0;
                // this.m_L1 = 0;
                // this.m_L2 = 0;
                // this.m_L3 = 0;
            }
        }

        private BitElement m_BitElement;
        public bool IsCreated => !this.m_BitElement.IsNull;

        public int Length => this.m_BitElement.Length;

        public void Add(int num) {
            var bucketIndex = num >> 6;
            var bitIndex = num & 0x3f;
            this.m_BitElement[bucketIndex] |= 1ul << bitIndex;
        }

        public void Add(in BitArray bitArray) {
            for (int i = 0, len = bitArray.Length; i < len; i++) {
                this.m_BitElement[i] |= bitArray.m_BitElement[i];
            }
        }

        public void Remove(int num) {
            var bucketIndex = num >> 6;
            var bitIndex = num & 0x3f;
            this.m_BitElement[bucketIndex] &= ~(1ul << bitIndex);
        }

        public void Remove(in BitArray bitArray) {
            for (int i = 0, len = bitArray.Length; i < len; i++) {
                this.m_BitElement[i] &= ~bitArray.m_BitElement[i];
            }
        }

        public void Clear() {
            this.m_BitElement.Clear();
        }

        public bool Contains(int num) {
            var bucketIndex = num >> 6;
            var bitIndex = num & 0x3f;
            return (this.m_BitElement[bucketIndex] & 1ul << bitIndex) != 0;
        }

        public bool ContainsAll(in BitArray bitArray) {
            for (int i = 0, len = bitArray.Length; i < len; i++) {
                if ((this.m_BitElement[i] & bitArray.m_BitElement[i]) != bitArray.m_BitElement[i]) {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsAny(in BitArray bitArray) {
            for (int i = 0, len = bitArray.Length; i < len; i++) {
                if ((this.m_BitElement[i] & bitArray.m_BitElement[i]) != 0) {
                    return true;
                }
            }

            return false;
        }
    }
}