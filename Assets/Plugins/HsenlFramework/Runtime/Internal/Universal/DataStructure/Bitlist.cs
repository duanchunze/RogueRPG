using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if UNSAFE_SERIALIZATION
using System.Runtime.InteropServices;
#endif

using System.Text;
using MemoryPack;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class BitListShowOfEnumAttribute : Attribute {
        public readonly Type enumType;

        public BitListShowOfEnumAttribute(Type enumType) {
            this.enumType = enumType;
        }

        public BitListShowOfEnumAttribute(string typeName) {
            this.enumType = AssemblyHelper.GetType(typeName);
        }
    }

    public interface IReadOnlyBitlist {
        internal ulong this[int bucketIndex] { get; set; }
        internal int BucketLength { get; }
        int Length { get; }
        bool Contains(int value);
        Bitlist.ContainsEnumerable Contains(IReadOnlyBitlist other);

        bool ContainsAll(IReadOnlyBitlist other);

        bool ContainsAny(IReadOnlyBitlist other);

        bool ContainsAny(IReadOnlyBitlist other, out int num);

        List<int> ToList();
    }

    // public interface IBitlist {
    //     int Length { get; }
    //     void Add(int value);
    //     void Add(IList<int> values);
    //     void Add(IBitlist other);
    //     void Set(IBitlist other);
    //     void Remove(int value);
    //     void Remove(IBitlist other);
    //     bool Contains(int value);
    //     Bitlist.ContainsEnumerable Contains(IBitlist other);
    //     bool ContainsAll(IBitlist other);
    //     bool ContainsAny(IBitlist other);
    //     bool ContainsAny(IBitlist other, out int num);
    //     List<int> ToList();
    // }

    // 位列表, 主要用于判存, 代替List<Enum> 这种, 速度极快
    // 注意: 不要用于大数值, 那不适合. 数值从0开始, 这样不会浪费内存.
    [Serializable]
    [MemoryPackable()]
    public
#if UNSAFE_SERIALIZATION // memory pack 不支持不安全代码
    unsafe
#endif
        partial class Bitlist : IEnumerable<int>, IReadOnlyBitlist {
        protected const int DefaultLength = 64;
        protected const int SizeOfElement = 8; // bits数组里存的元素, 每个元素占几个字节大小
        protected const int NumberOfBits = 64; // bits数组里存的元素, 是多少位数的

        protected const ulong One = 1L;

#if UNSAFE_SERIALIZATION
        [MemoryPackInclude]
        protected ulong* ptr;
#endif

        [MemoryPackInclude]
        protected ulong[] bits;

        [MemoryPackInclude]
        protected int bucketLength;

        [MemoryPackInclude]
        protected int capacity;

        [MemoryPackIgnore]
        int IReadOnlyBitlist.BucketLength => this.bucketLength;

        [MemoryPackIgnore]
        // Length总是64的倍数
        public int Length => this.bucketLength * NumberOfBits;

        public static bool IsNullOrEmpty(Bitlist bitlist) {
            if (bitlist == null) return true;
            for (int i = 0, len = bitlist.bucketLength; i < len; i++) {
                if (bitlist[i] != 0) {
                    return false;
                }
            }

            return true;
        }

        public Bitlist() : this(DefaultLength) { }

#if UNSAFE_SERIALIZATION
        // capacity每次拓展都是64的倍数
        [MemoryPackConstructor]
        public BitList(int capacity) {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(capacity.ToString());
            this.capacity = capacity;
            capacity--;
            this.arrayLength = capacity / NumberOfBits + 1;
            this.ptr = (ulong*)Marshal.AllocHGlobal(this.arrayLength * SizeOfElement);
            this.Clear(); // Marshal.AllocHGlobal分配的内存是不保证内存为空的, 所以, 刚申请的内存需要清理一下
            GC.AddMemoryPressure(this.arrayLength * SizeOfElement);
        }
#endif

        // capacity每次拓展都是64的倍数
        [MemoryPackConstructor]
        public Bitlist(int capacity) {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(capacity.ToString());
            this.capacity = capacity;
            capacity--;
            this.bucketLength = capacity / NumberOfBits + 1;
            this.bits = new ulong[this.bucketLength];
        }

        // 这里获取的并不是每个位的值, 而是ulong[]的元素值.
        // 为什么不像列表那样, 根据索引获取值呢?
        // 在位列表中, 这没有实际意义, 因为位列表每一位上实际上只有0或1, 所以如果想知道某一个索引的值, 直接用Contains(int index)就好了.
        // 其实, 位列表的主要用途是用来判存的, 所以单纯的获取某位上的值意义也不大
#if UNSAFE_SERIALIZATION
        internal ulong this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *(this.ptr + bucketIndex);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => *(this.ptr + bucketIndex) = value;
        }
        
        ulong IReadOnlyBitlist.this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => *(this.ptr + bucketIndex);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => *(this.ptr + bucketIndex) = value;
        }
#endif

        internal ulong this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.bits[bucketIndex];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.bits[bucketIndex] = value;
        }

        ulong IReadOnlyBitlist.this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.bits[bucketIndex];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.bits[bucketIndex] = value;
        }

        public void Add(int value) {
            if (value < 0)
                throw new ArgumentOutOfRangeException(value.ToString());

            if (value > 1024) Log.Warning("bit list add num isn't that a little big? -- " + value); // 一般来说, 位列表作为给枚举判存, 1024个枚举元素已经够大了
            var bucketIndex = value >> 6; // 等价于 i / 64, int 则为 >> 5
            var bitIndex = value & 0x3f; // 等价于 i % 64, int 则为 & 0x1f
            this.Resize(bucketIndex + 1);
            this[bucketIndex] |= One << bitIndex;
        }

        public void Add(IList<int> values) {
            for (int i = 0, len = values.Count; i < len; i++) {
                this.Add(values[i]);
            }
        }

        public void Add(Bitlist other) {
            this.Resize(other.bucketLength);
            for (var i = 0; i < other.bucketLength; i++) {
                this[i] |= other[i];
            }
        }

#if UNSAFE_SERIALIZATION
        public void Set(BitList other) {
            this.Clear();
            Buffer.MemoryCopy(other.ptr, this.ptr, other.arrayLength, other.arrayLength);
        }
#endif

        public void Set(Bitlist other) {
            this.Clear();
            var len = Math.Min(this.bits.Length, other.bits.Length);
            Array.Copy(other.bits, this.bits, len);
        }

        public void Remove(int value) {
            var bucketIndex = value >> 6;
            var bucketLen = bucketIndex + 1;
            if (this.bucketLength < bucketLen) {
                return;
            }

            var bitIndex = value & 0x3f;
            this[bucketIndex] &= ~(One << bitIndex);
        }

        public void Remove(Bitlist other) {
            for (int i = 0, len = other.bucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    break;
                }

                this[i] &= ~other[i];
            }
        }

        public bool Contains(int value) {
            if (value < 0)
                throw new ArgumentOutOfRangeException(value.ToString());

            var bucketIndex = value >> 6;
            var bucketLen = bucketIndex + 1;
            if (this.bucketLength < bucketLen) {
                return false;
            }

            var bitIndex = value & 0x3f;
            return (this[bucketIndex] & (One << bitIndex)) != 0;
        }

        public ContainsEnumerable Contains(IReadOnlyBitlist other) {
            return this.Contains((Bitlist)other);
        }

        public bool ContainsAll(IReadOnlyBitlist other) {
            for (int i = 0, len = other.BucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    return false;
                }

                if ((this[i] & other[i]) != other[i]) {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsAny(IReadOnlyBitlist other) {
            for (int i = 0, len = other.BucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    break;
                }

                var orv = this[i] & other[i];
                if (orv != 0) {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsAny(IReadOnlyBitlist other, out int num) {
            num = 0;
            for (int i = 0, len = other.BucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    break;
                }

                var orv = this[i] & other[i];
                if (orv != 0) {
                    num = this.GetBitPosition(orv) + i * NumberOfBits;
                    return true;
                }
            }

            return false;
        }

        // 挑出所有两个位列表同时存在的数
        public ContainsEnumerable Contains(Bitlist other) => new(this, other);

        // 必须包含 other 中所有的值, 才算 true
        public bool ContainsAll(Bitlist other) {
            for (int i = 0, len = other.bucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    return false;
                }

                if ((this[i] & other[i]) != other[i]) {
                    return false;
                }
            }

            return true;
        }

        // 只要有任意存在即为true
        public bool ContainsAny(Bitlist other) {
            for (int i = 0, len = other.bucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    break;
                }

                var orv = this[i] & other[i];
                if (orv != 0) {
                    return true;
                }
            }

            return false;
        }

        // 判存, 只要存在即刻返回, 且提供该数
        public bool ContainsAny(Bitlist other, out int num) {
            num = 0;
            for (int i = 0, len = other.bucketLength, selfLen = this.bucketLength; i < len; i++) {
                if (i > selfLen - 1) {
                    break;
                }

                var orv = this[i] & other[i];
                if (orv != 0) {
                    num = this.GetBitPosition(orv) + i * NumberOfBits;
                    return true;
                }
            }

            return false;
        }
        
        // 获得二进制位为1的位置
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int GetBitPosition(ulong value) {
            var mask = 0xffffffffffffffff;
            var position_r = 0;
            var position_l = 0;
            var counter = 1;
            const int maxCounter = 7; // ulong是64位, 总共可以对折6次, 所以是1 + 6 = 7

            while (counter < maxCounter) {
                var bitnum = NumberOfBits >> counter;
                var tempmask = mask >> (bitnum + position_r);
                tempmask <<= position_r;
                var orv = value & tempmask;
                if (orv != 0) {
                    position_l += bitnum;
                    mask = tempmask;
                }
                else {
                    position_r += bitnum;
                    mask <<= (bitnum + position_l);
                    mask >>= position_l;
                }

                counter++;
            }

            // pr和pl是相对关系, pl = 63 - pr
            return position_r;
        }

        public ForeachEnumerator GetEnumerator() => new(this);

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ForeachEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new ForeachEnumerator(this);

#if UNSAFE_SERIALIZATION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resize(int size) {
            if (size < this.arrayLength) return;
            GC.RemoveMemoryPressure(this.arrayLength * SizeOfElement);
            var oldLength = this.arrayLength;
            this.arrayLength = size;
            var cb = size * SizeOfElement;
            this.ptr = (ulong*)Marshal.ReAllocHGlobal((IntPtr)this.ptr, new IntPtr(cb));
            // Marshal.ReAllocHGlobal方法不会确保分配的内存一定是初始化过的, 所以, 我们拿到这段新内存时, 要自己初始化下
            for (var i = oldLength; i < size; i++) {
                this[i] = 0;
            }

            GC.AddMemoryPressure(cb);
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resize(int size) {
            if (size < this.bucketLength) return;
            this.bucketLength = size;
            var newBits = new ulong[size];
            Array.Copy(this.bits, newBits, this.bits.Length);
            this.bits = newBits;
        }

        public void Clear() {
            for (var i = 0; i < this.bucketLength; i++) {
                this[i] = 0;
            }
        }

        public List<int> ToList() {
            List<int> list = new();
            for (var i = 0; i < this.bucketLength; i++) {
                for (var j = 0; j < NumberOfBits; j++) {
                    if ((this[i] & (One << j)) != 0) {
                        list.Add(i * NumberOfBits + j);
                    }
                }
            }

            return list;
        }

        public override string ToString() {
            var list = this.ToList();
            StringBuilder builder = new();
            for (int i = 0, len = list.Count; i < len; i++) {
                builder.Append(list[i]);
                if (i == len - 1) break;
                builder.Append(',');
            }

            return builder.ToString();
        }

#if UNSAFE_SERIALIZATION
        ~BitList() {
            Marshal.FreeHGlobal((nint)this.ptr);
            GC.RemoveMemoryPressure(this.arrayLength * SizeOfElement);
        }
#endif

        public readonly struct ContainsEnumerable : IEnumerable<int> {
            private readonly Bitlist _list1;
            private readonly Bitlist _list2;

            public ContainsEnumerable(Bitlist list1, Bitlist list2) {
                this._list1 = list1;
                this._list2 = list2;
            }

            public ContainsEnumerator GetEnumerator() => new(this._list1, this._list2);
            IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ContainsEnumerator(this._list1, this._list2);
            IEnumerator IEnumerable.GetEnumerator() => new ContainsEnumerator(this._list1, this._list2);
        }

        public struct ContainsEnumerator : IEnumerator<int> {
            private Bitlist _lhs;
            private Bitlist _rhs;

            public int Current { get; private set; }

            public ContainsEnumerator(Bitlist l, Bitlist r) {
                this._lhs = l;
                this._rhs = r;
                this.Current = -1;
            }

            public bool MoveNext() {
                this.Current++;
                var bucketIndex = this.Current >> 6;
                var bitIndex = this.Current & 0x3f;
                for (int i = bucketIndex, leftLen = this._lhs.bucketLength, rightLen = this._rhs.bucketLength; i < leftLen && i < rightLen; i++, bitIndex = 0) {
                    var bit1 = this._lhs[i];
                    var bit2 = this._rhs[i];
                    var orv = bit1 & bit2;
                    if (orv == 0) continue;
                    for (var j = bitIndex; j < NumberOfBits; j++) {
                        if ((orv & (One << j)) != 0) {
                            this.Current = i * NumberOfBits + j;
                            return true;
                        }
                    }
                }

                return false;
            }

            public void Reset() {
                this._lhs = null;
                this._rhs = null;
                this.Current = -1;
            }

            object IEnumerator.Current => this.Current;

            public void Dispose() {
                this.Reset();
            }
        }

        // disable code prompt
        public struct ForeachEnumerator : IEnumerator<int> {
            private Bitlist _list;

            public int Current { get; private set; }

            public ForeachEnumerator(Bitlist list) {
                this._list = list;
                this.Current = -1;
            }

            public bool MoveNext() {
                this.Current++;
                var bucketIndex = this.Current >> 6;
                var bitIndex = this.Current & 0x3f;
                for (int i = bucketIndex, len = this._list.bucketLength; i < len; i++, bitIndex = 0) {
                    var bit = this._list[i];
                    if (bit == 0) continue;
                    for (var j = bitIndex; j < NumberOfBits; j++) {
                        if ((bit & (One << j)) != 0) {
                            this.Current = i * NumberOfBits + j;
                            return true;
                        }
                    }
                }

                return false;
            }

            public void Reset() {
                this._list = null;
                this.Current = -1;
            }

            object IEnumerator.Current => this.Current;

            public void Dispose() { }
        }
    }

    public static class BitlistExtension {
        public static bool IsNullOrEmpty(this Bitlist self) {
            return Bitlist.IsNullOrEmpty(self);
        }

        public static bool IsNullOrEmpty(this IReadOnlyBitlist self) {
            if (self is Bitlist bitlist) {
                return Bitlist.IsNullOrEmpty(bitlist);
            }

            return true;
        }

        public static void Add<T>(this Bitlist self, T enu) where T : Enum {
            self.Add(enu.GetHashCode());
        }

        public static void Add<T>(this Bitlist self, IList<T> enums) where T : Enum {
            for (int i = 0, len = enums.Count; i < len; i++) {
                self.Add(enums[i].GetHashCode());
            }
        }

        public static void Remove<T>(this Bitlist self, T enu) where T : Enum {
            self.Remove(enu.GetHashCode());
        }

        public static void Set<T>(this Bitlist self, IList<T> enums) where T : Enum {
            self.Clear();
            for (int i = 0, len = enums.Count; i < len; i++) {
                self.Add(enums[i].GetHashCode());
            }
        }

        public static bool Contains<T>(this Bitlist self, T e) where T : Enum {
            return self.Contains(e.GetHashCode());
        }
    }
}