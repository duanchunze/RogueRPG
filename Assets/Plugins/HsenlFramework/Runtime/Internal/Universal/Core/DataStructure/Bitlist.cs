using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

#if UNSAFE_SERIALIZATION
using System.Runtime.InteropServices;
#endif

using System.Text;
using MemoryPack;

// ReSharper disable NonReadonlyMemberInGetHashCode

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

        int BucketLength { get; }

        int Length { get; }

        bool Contains(int value);

        Bitlist.ContainsEnumerable Contains(IReadOnlyBitlist other);

        bool ContainsAll(IReadOnlyBitlist other);

        bool ContainsAny(IReadOnlyBitlist other);

        bool ContainsAny(IReadOnlyBitlist other, out int num);

        List<int> ToList();
    }

    public static class BitlistAssistant {
        // 预算出byte范围内, 每个数有多少个二进制的1, 同时每个数也是其index
        private static readonly int[] _lookupTable = new int[256];

        static BitlistAssistant() {
            for (var i = 0; i < 256; i++) {
                //   0   1   2   3   4   5   6   7
                // 000 001 010 011 100 101 110 111
                // 把这个数分两个情况看, 一个是偶数还是奇数, 偶数为0, 奇数为1, 第二个看这个数能塞下几个2 4 8 16 32....这些数, 能塞下几个就计数几个
                // 最后把二者相加
                _lookupTable[i] = (i & 1) + _lookupTable[i / 2];
            }
        }

        // 获得二进制位为1的位置
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBitPosition(ulong value) {
            var mask = 0xffffffffffffffff;
            var position_r = 0;
            var position_l = 0;
            var counter = 1;
            const int maxCounter = 7; // ulong是64位, 总共可以对折6次, 所以是1 + 6 = 7

            while (counter < maxCounter) {
                var bitnum = Bitlist.NumOfBits >> counter;
                var tempmask = mask >> (bitnum + position_r);
                tempmask <<= position_r;
                var orv = value & tempmask;
                if (orv != 0) {
                    position_l += bitnum;
                    mask = tempmask;
                }
                else {
                    position_r += bitnum;
                    mask <<= bitnum + position_l;
                    mask >>= position_l;
                }

                counter++;
            }

            // pr和pl是相对关系, pl = 63 - pr
            return position_r;
        }

        // 一个数作为二进制时, 有多少个1
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetBitOneCount(ulong value) {
            var count = 0;
            while (value > 0) {
                count += _lookupTable[value & 0xff];
                value >>= 8;
            }

            return count;
        }
    }

    // 位列表, 主要用于判存, 代替List<Enum> 这种, 速度极快
    // 注意: 不要用于大数值, 那不适合. 数值从0开始, 这样不会浪费内存.
    [Serializable]
    [MemoryPackable()]
    public
#if UNSAFE_SERIALIZATION // memory pack 不支持不安全代码
        unsafe
#endif
        partial class Bitlist : IEnumerable<int>, IReadOnlyBitlist, IEquatable<Bitlist> {
        public const int DefaultLength = 64;
        public const int SizeOfElement = 8; // masks数组里存的元素, 每个元素占几个字节大小
        public const int NumOfBits = 64; // masks数组里存的元素, 是多少位数的

        public const ulong One = 1L;


#if UNSAFE_SERIALIZATION
        [MemoryPackInclude]
        protected ulong* ptr;
#endif

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackInclude]
        protected ulong[] masks;

        [MemoryPackInclude]
        protected int capacity;

#if UNSAFE_SERIALIZATION
        [MemoryPackInclude]
        protected int bucketLength;
        
        [MemoryPackIgnore]
        public int BucketLength => this.bucketLength;
#else
        [MemoryPackIgnore]
        public int BucketLength => this.masks.Length;
#endif

        [MemoryPackIgnore]
        // Length总是64的倍数
        public int Length => this.BucketLength * NumOfBits;

        public static bool IsNullOrEmpty(Bitlist bitlist) {
            if (bitlist == null) return true;
            for (int i = 0, len = bitlist.BucketLength; i < len; i++) {
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
            this.bucketLength = capacity / NumOfBits + 1;
            var bytelen = this.bucketLength * SizeOfElement;
            this.ptr = (ulong*)Marshal.AllocHGlobal(bytelen);
            GC.AddMemoryPressure(this.bucketLength * SizeOfElement);
            // Marshal.AllocHGlobal分配的内存是不保证内存为空的, 所以, 刚申请的内存需要清理一下
            Unsafe.InitBlockUnaligned(this.ptr, 0, (uint)bytelen);
        }
#else
        // capacity每次拓展都是64的倍数
        [MemoryPackConstructor]
        public Bitlist(int capacity) {
            if (capacity <= 0) throw new ArgumentOutOfRangeException(capacity.ToString());
            this.capacity = capacity;
            capacity--;
            this.masks = new ulong[capacity / NumOfBits + 1];
        }
#endif

        // 这里获取的并不是每个位的值, 而是ulong[]的元素值.
        // 为什么不像列表那样, 根据索引获取值呢?
        // 在位列表中, 这没有实际意义, 因为位列表每一位上实际上只有0或1, 所以如果想知道某一个索引的值, 直接用Contains(int index)就好了.
        // 其实, 位列表的主要用途是用来判存的, 所以单纯的获取某位上的值意义也不大
#if UNSAFE_SERIALIZATION
        internal ulong this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (bucketIndex >= this.BucketLength) {
                    throw new IndexOutOfRangeException(bucketIndex.ToString());
                }

                return *(this.ptr + bucketIndex);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (bucketIndex >= this.BucketLength) {
                    throw new IndexOutOfRangeException(bucketIndex.ToString());
                }

                *(this.ptr + bucketIndex) = value;
            }
        }

        ulong IReadOnlyBitlist.this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                if (bucketIndex >= this.BucketLength) {
                    throw new IndexOutOfRangeException(bucketIndex.ToString());
                }

                return *(this.ptr + bucketIndex);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set {
                if (bucketIndex >= this.BucketLength) {
                    throw new IndexOutOfRangeException(bucketIndex.ToString());
                }

                *(this.ptr + bucketIndex) = value;
            }
        }
#else
        internal ulong this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.masks[bucketIndex];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.masks[bucketIndex] = value;
        }

        ulong IReadOnlyBitlist.this[int bucketIndex] {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.masks[bucketIndex];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => this.masks[bucketIndex] = value;
        }
#endif

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
            this.Resize(other.BucketLength);
            for (var i = 0; i < other.BucketLength; i++) {
                this[i] |= other[i];
            }
        }

#if UNSAFE_SERIALIZATION
        public void Set(Bitlist other) {
            this.Resize(other.BucketLength, false);
            Buffer.MemoryCopy(other.ptr, this.ptr, this.BucketLength, other.BucketLength);
        }
#else
        public void Set(Bitlist other) {
            this.Resize(other.BucketLength, false);
            Array.Copy(other.masks, this.masks, other.masks.Length);
        }
#endif

        public void Remove(int value) {
            if (value < 0)
                throw new ArgumentOutOfRangeException(value.ToString());

            var bucketIndex = value >> 6;
            var bucketLen = bucketIndex + 1;
            if (this.BucketLength < bucketLen) {
                return;
            }

            var bitIndex = value & 0x3f;
            this[bucketIndex] &= ~(One << bitIndex);
        }

        public void Remove(Bitlist other) {
            for (int i = 0, len = other.BucketLength, selfLen = this.BucketLength; i < len; i++) {
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
            if (this.BucketLength < bucketLen) {
                return false;
            }

            var bitIndex = value & 0x3f;
            return (this[bucketIndex] & (One << bitIndex)) != 0;
        }

        public ContainsEnumerable Contains(IReadOnlyBitlist other) {
            return this.Contains((Bitlist)other);
        }

        public bool ContainsAll(IReadOnlyBitlist other) {
            // 对比分两种情况, 一种是other 的 bucket len比自己短, 那就正常遍历完other就行了, 另一种是other 的 bucket len 比自己长, 那就要判断比自己长的部分, 是否为空, 只有全部为
            // 空, 才算true
            var maxIndex = this.BucketLength - 1;
            for (int i = 0, len = other.BucketLength; i < len; i++) {
                if (i > maxIndex) {
                    if (other[i] != 0) {
                        return false;
                    }
                }

                if ((this[i] & other[i]) != other[i]) {
                    return false;
                }
            }

            return true;
        }

        public bool ContainsAny(IReadOnlyBitlist other) {
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
                var orv = this[i] & other[i];
                if (orv != 0) {
                    return true;
                }
            }

            return false;
        }

        public bool ContainsAny(IReadOnlyBitlist other, out int num) {
            num = 0;
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
                var orv = this[i] & other[i];
                if (orv != 0) {
                    num = BitlistAssistant.GetBitPosition(orv) + i * NumOfBits;
                    return true;
                }
            }

            return false;
        }

        public int ContainsCount(IReadOnlyBitlist other) {
            var count = 0;
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
                var orv = this[i] & other[i];
                if (orv != 0) {
                    count += BitlistAssistant.GetBitOneCount(orv);
                }
            }

            return count;
        }

        // 挑出所有两个位列表同时存在的数
        public ContainsEnumerable Contains(Bitlist other) => new(this, other);

        // 必须包含 other 中所有的值, 才算 true
        public bool ContainsAll(Bitlist other) {
            // 对比分两种情况, 一种是other 的 bucket len比自己短, 那就正常遍历完other就行了, 另一种是other 的 bucket len 比自己长, 那就要判断比自己长的部分, 是否为空, 只有全部为
            // 空, 才算true
            var maxIndex = this.BucketLength - 1;
            for (int i = 0, len = other.BucketLength; i < len; i++) {
                if (i > maxIndex) {
                    if (other[i] != 0) {
                        return false;
                    }
                }

                if ((this[i] & other[i]) != other[i]) {
                    return false;
                }
            }

            return true;
        }

        // 只要有任意存在即为true
        public bool ContainsAny(Bitlist other) {
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
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
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
                var orv = this[i] & other[i];
                if (orv != 0) {
                    num = BitlistAssistant.GetBitPosition(orv) + i * NumOfBits;
                    return true;
                }
            }

            return false;
        }

        /// 判断二者有多少个共存位
        public int ContainsCount(Bitlist other) {
            var count = 0;
            var min = Math.Min(other.BucketLength, this.BucketLength);
            for (int i = 0; i < min; i++) {
                var orv = this[i] & other[i];
                if (orv != 0) {
                    count += BitlistAssistant.GetBitOneCount(orv);
                }
            }

            return count;
        }

        public ForeachEnumerator GetEnumerator() => new(this);

        IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ForeachEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => new ForeachEnumerator(this);

#if UNSAFE_SERIALIZATION
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resize(int bucketLen, bool onlyExpand = true) {
            if (bucketLen == this.bucketLength) return;
            if (bucketLen < this.bucketLength && onlyExpand) return;

            var oldLen = this.bucketLength;
            this.bucketLength = bucketLen;
            var cb = bucketLen * SizeOfElement;
            // 重新分配内存, 多了剪裁掉(从后面剪), 少了扩张, 如果ptr为空, 默认会重新分配一个ptr
            this.ptr = (ulong*)Marshal.ReAllocHGlobal((nint)this.ptr, (nint)cb);

            if (bucketLen > oldLen) {
                GC.AddMemoryPressure((bucketLen - oldLen) * SizeOfElement);
                // Marshal.ReAllocHGlobal方法不能保证分配的内存一定是初始化过的, 所以, 我们拿到这段新内存时, 要自己初始化下
                for (var i = oldLen; i < bucketLen; i++) {
                    this[i] = 0;
                }
            }
            else {
                GC.RemoveMemoryPressure((oldLen - bucketLen) * SizeOfElement);
            }
        }
#else
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Resize(int bucketLen, bool onlyExpand = true) {
            if (bucketLen == this.BucketLength) return;
            if (bucketLen < this.BucketLength && onlyExpand) return;

            var newBits = new ulong[bucketLen];
            var copylen = bucketLen > this.BucketLength ? this.BucketLength : bucketLen;
            Array.Copy(this.masks, newBits, copylen);
            this.masks = newBits;
        }
#endif

        public void Clear() {
            for (var i = 0; i < this.BucketLength; i++) {
                this[i] = 0;
            }
        }

#if UNSAFE_SERIALIZATION
        public void ClearThorough() {
            if (this.ptr != null) {
                Marshal.FreeHGlobal((nint)this.ptr);
                GC.RemoveMemoryPressure(this.bucketLength * SizeOfElement);
                this.ptr = null;
            }
        }
#else
        public void ClearThorough() {
            this.masks = Array.Empty<ulong>();
        }
#endif

        public List<int> ToList() {
            List<int> list = new();
            for (var i = 0; i < this.BucketLength; i++) {
                for (var j = 0; j < NumOfBits; j++) {
                    if ((this[i] & (One << j)) != 0) {
                        list.Add(i * NumOfBits + j);
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
            this.ClearThorough();
        }
#endif

        public bool Equals(Bitlist other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            // 只要内核相等, 就算相等, 其他的两个字段忽略
            if (this.BucketLength != other.BucketLength) return false;
            for (int i = 0, len = this.BucketLength; i < len; i++) {
                if (this[i] != other[i])
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj) {
            if (obj is not Bitlist bitlist) return false;
            return this.Equals(bitlist);
        }

        public override int GetHashCode() {
            unchecked {
                var hash = 17;
                hash = hash * 23 + this.capacity.GetHashCode();
                hash = hash * 23 + this.BucketLength.GetHashCode();
                if (this.masks != null) {
                    foreach (var item in this.masks) {
                        hash = hash * 23 + item.GetHashCode();
                    }
                }

                return hash;
            }
        }

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
            private int _minBucketLen;

            public int Current { get; private set; }

            public ContainsEnumerator(Bitlist l, Bitlist r) {
                this._lhs = l;
                this._rhs = r;
                this.Current = -1;
                this._minBucketLen = Math.Min(this._lhs.BucketLength, this._rhs.BucketLength);
            }

            public bool MoveNext() {
                this.Current++;
                var bucketIndex = this.Current >> 6;
                var bitIndex = this.Current & 0x3f;
                for (int i = bucketIndex; i < this._minBucketLen; i++, bitIndex = 0) {
                    var bit1 = this._lhs[i];
                    var bit2 = this._rhs[i];
                    var orv = bit1 & bit2;
                    if (orv == 0) continue;
                    for (var j = bitIndex; j < NumOfBits; j++) {
                        if ((orv & (One << j)) != 0) {
                            this.Current = i * NumOfBits + j;
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
                this._minBucketLen = 0;
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
                for (int i = bucketIndex, len = this._list.BucketLength; i < len; i++, bitIndex = 0) {
                    var bit = this._list[i];
                    if (bit == 0) continue;
                    for (var j = bitIndex; j < NumOfBits; j++) {
                        if ((bit & (One << j)) != 0) {
                            this.Current = i * NumOfBits + j;
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