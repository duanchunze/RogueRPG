// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using System.Text;
// using MemoryPack;
// using UnityEngine;
//
// namespace Hsenl {
//     // 用于odin的版本, 但unity也可以使用
//     [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
//     public class BitListShowOfEnumAttribute : Attribute {
//         public readonly Type enumType;
//
//         public BitListShowOfEnumAttribute(Type enumType) {
//             this.enumType = enumType;
//         }
//     }
//
//     // 用于unity的版本
//     // [AttributeUsage(AttributeTargets.Field)]
//     // public class BitListShowOfEnumAttribute : PropertyDrawer {
//     //     public readonly Type enumType;
//     //
//     //     public BitListShowOfEnumAttribute(Type enumType) {
//     //         this.enumType = enumType;
//     //     }
//     // }
//
//     public interface IReadOnlyBitList {
//         bool Contains(int num);
//         bool ContainsAll(BitList list);
//         bool ContainsAny(BitList list);
//         bool ContainsAny(BitList list, out int index);
//         List<int> ToList();
//     }
//
//     public interface IBitList : IReadOnlyBitList {
//         void Add(int value);
//         void Remove(int value);
//     }
//
//     /// <summary>
//     /// 位列表
//     /// 适合用于 enum判存，速度与内存都比直接使用 List`enum好的多
//     /// 但需要特别注意：使用时，enum一定要从0开始，不然会造成内存浪费，比如，如果从1000开始，那么就会浪费1000/64，大概10个int大小的内存
//     /// </summary>
//     [Serializable]
//     [MemoryPackable]
//     public partial class BitList : IBitList, IReadOnlyBitList, IEnumerable<int> {
//         protected const int DefaultLength = 64;
//         protected const int SizeOfElement = 8;
//         protected const int BitOfElement = 64;
//
//         [SerializeField]
//         [MemoryPackInclude]
//         protected ulong[] bits;
//         
//         [MemoryPackInclude]
//         protected int arrayLength;
//
//         [MemoryPackInclude]
//         protected int capacity;
//
//         [MemoryPackIgnore]
//         // Length总是64的倍数
//         public int Length => this.arrayLength * BitOfElement;
//
//         public static bool IsNullOrEmpty(BitList bitList) {
//             if (bitList?.bits == null) return true;
//
//             for (int i = 0, len = bitList.bits.Length; i < len; i++) {
//                 if (bitList.bits[i] != 0) {
//                     return false;
//                 }
//             }
//
//             return true;
//         }
//
//         [MemoryPackConstructor]
//         public BitList() : this(DefaultLength) { }
//
//         public BitList(int capacity) {
//             if (capacity <= 0) throw new ArgumentOutOfRangeException(capacity.ToString());
//             this.capacity = capacity;
//             capacity--;
//             this.arrayLength = capacity / 64 + 1;
//             this.bits = new ulong[this.arrayLength];
//         }
//         
//         public void Add(int num) {
//             if (num > 1024) Log.Warning("bit list add num isn't that a little big? -- " + num); // 一般来说, 位列表作为给枚举判存, 1024个枚举元素已经够大了
//             var bucketIndex = num >> 6; // 等价于 i / 64, int 则为 >> 5
//             var bitIndex = num & 0x3f; // 等价于 i % 64, int 则为 & 0x1f
//             this.Resize(bucketIndex + 1);
//             this.bits[bucketIndex] |= 1ul << bitIndex;
//         }
//
//         public void Add(IList<int> nums) {
//             for (int i = 0, len = nums.Count; i < len; i++) {
//                 this.Add(nums[i]);
//             }
//         }
//
//         public void Add(BitList list) {
//             if (list.bits == null) {
//                 return;
//             }
//
//             this.Resize(list.bits.Length);
//             for (int i = 0, len = list.bits.Length; i < len; i++) {
//                 this.bits[i] |= list.bits[i];
//             }
//         }
//
//         // 直接设置，设置前，会先清空
//         public void Set(BitList list) {
//             if (list.bits == null) {
//                 return;
//             }
//
//             this.Clear();
//             this.Add(list);
//         }
//
//         public void Remove(int num) {
//             if (this.bits == null) {
//                 return;
//             }
//
//             var bucketIndex = num >> 6;
//             var bucketLen = bucketIndex + 1;
//             if (this.bits.Length < bucketLen) {
//                 return;
//             }
//
//             var bitIndex = num & 0x3f;
//             this.bits[bucketIndex] &= ~(1ul << bitIndex);
//         }
//
//         public void Remove(BitList list) {
//             if (this.bits == null || list?.bits == null) {
//                 return;
//             }
//
//             for (int i = 0, len = list.bits.Length, selfLen = this.bits.Length; i < len; i++) {
//                 if (i > selfLen - 1) {
//                     break;
//                 }
//
//                 this.bits[i] &= ~list.bits[i];
//             }
//         }
//
//         public bool Contains(int num) {
//             if (this.bits == null) {
//                 return false;
//             }
//
//             var bucketIndex = num >> 6;
//             var bucketLen = bucketIndex + 1;
//             if (this.bits.Length < bucketLen) {
//                 return false;
//             }
//
//             var bitIndex = num & 0x3f;
//             return (this.bits[bucketIndex] & (1ul << bitIndex)) != 0;
//         }
//
//         // 挑出所有两个位列表同时存在的数
//         public ContainsEnumerable Contains(BitList list) => new(this, list);
//
//         // 当前位列表, 必须全部匹配目标位列表里的所有数
//         public bool ContainsAll(BitList list) {
//             if (this.bits == null || list?.bits == null) {
//                 return false;
//             }
//
//             for (int i = 0, len = list.bits.Length, selfLen = this.bits.Length; i < len; i++) {
//                 if (i > selfLen - 1) {
//                     return false;
//                 }
//
//                 if ((this.bits[i] & list.bits[i]) != list.bits[i]) {
//                     return false;
//                 }
//             }
//
//             return true;
//         }
//
//         // 当前位列表, 只需要匹配目标位列表里的任意一个数
//         public bool ContainsAny(BitList list) {
//             if (this.bits == null || list?.bits == null) {
//                 return false;
//             }
//
//             for (int i = 0, len = list.bits.Length, selfLen = this.bits.Length; i < len; i++) {
//                 if (i > selfLen - 1) {
//                     break;
//                 }
//
//                 var orv = this.bits[i] & list.bits[i];
//                 if (orv != 0) {
//                     return true;
//                 }
//             }
//
//             return false;
//         }
//
//         public bool ContainsAny(BitList list, out int idx) {
//             idx = 0;
//             if (this.bits == null || list?.bits == null) {
//                 return false;
//             }
//
//             for (int i = 0, len = list.bits.Length, selfLen = this.bits.Length; i < len; i++) {
//                 if (i > selfLen - 1) {
//                     break;
//                 }
//
//                 var orv = this.bits[i] & list.bits[i];
//                 if (orv != 0) {
//                     idx = GetIndex(orv) + i * BitOfElement;
//                     return true;
//                 }
//             }
//
//             return false;
//         }
//
//         public ForeachEnumerator GetEnumerator() => new(this);
//
//         IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ForeachEnumerator(this);
//
//         IEnumerator IEnumerable.GetEnumerator() => new ForeachEnumerator(this);
//
//         public List<int> ToList() {
//             List<int> list = new();
//             if (this.bits == null) return list;
//             for (var i = 0; i < this.bits.Length; i++) {
//                 for (var j = 0; j < BitOfElement; j++) {
//                     if ((this.bits[i] & (1ul << j)) != 0) {
//                         list.Add(i * BitOfElement + j);
//                     }
//                 }
//             }
//
//             return list;
//         }
//
//         public void Clear() {
//             if (this.bits == null) {
//                 return;
//             }
//
//             if (this.bits.Length > 4) {
//                 this.bits = null;
//                 return;
//             }
//
//             for (int i = 0, len = this.bits.Length; i < len; i++) {
//                 this.bits[i] = 0;
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         protected static int GetIndex(ulong num) {
//             var result = 0;
//             while (num >> result++ > 0) { }
//
//             return result - 2;
//         }
//
//         protected void Resize(int cap) {
//             if (this.bits == null) {
//                 this.bits = new ulong[cap];
//             }
//             else {
//                 if (this.bits.Length < cap) {
//                     var copy = this.bits;
//                     this.bits = new ulong[cap];
//                     for (var j = 0; j < copy.Length; j++) {
//                         this.bits[j] = copy[j];
//                     }
//                 }
//             }
//         }
//
//         public override string ToString() {
//             var list = this.ToList();
//             StringBuilder builder = new();
//             for (int i = 0, len = list.Count; i < len; i++) {
//                 builder.Append(list[i]);
//                 if (i == len - 1) break;
//                 builder.Append(',');
//             }
//
//             return builder.ToString();
//         }
//
//         public struct ContainsEnumerable : IEnumerable<int> {
//             private readonly BitList _list1;
//             private readonly BitList _list2;
//
//             public ContainsEnumerable(BitList list1, BitList list2) {
//                 this._list1 = list1;
//                 this._list2 = list2;
//             }
//
//             public ContainsEnumerator GetEnumerator() => new(this._list1, this._list2);
//             IEnumerator<int> IEnumerable<int>.GetEnumerator() => new ContainsEnumerator(this._list1, this._list2);
//             IEnumerator IEnumerable.GetEnumerator() => new ContainsEnumerator(this._list1, this._list2);
//         }
//
//         public struct ContainsEnumerator : IEnumerator<int> {
//             private BitList _lhs;
//             private BitList _rhs;
//
//             public int Current { get; private set; }
//
//             public ContainsEnumerator(BitList l, BitList r) {
//                 this._lhs = l;
//                 this._rhs = r;
//                 this.Current = -1;
//             }
//
//             public bool MoveNext() {
//                 if (this._lhs.bits == null || this._rhs?.bits == null) return false;
//                 this.Current++;
//                 var bucketIndex = this.Current >> 6;
//                 var bitIndex = this.Current & 0x3f;
//                 for (int i = bucketIndex, leftLen = this._lhs.bits.Length, rightLen = this._rhs.bits.Length; i < leftLen && i < rightLen; i++, bitIndex = 0) {
//                     var bit1 = this._lhs.bits[i];
//                     var bit2 = this._rhs.bits[i];
//                     var orv = bit1 & bit2;
//                     if (orv == 0) continue;
//                     for (var j = bitIndex; j < BitOfElement; j++) {
//                         if ((orv & (1ul << j)) != 0) {
//                             this.Current = i * BitOfElement + j;
//                             return true;
//                         }
//                     }
//                 }
//
//                 return false;
//             }
//
//             public void Reset() {
//                 this._lhs = null;
//                 this._rhs = null;
//                 this.Current = -1;
//             }
//
//             object IEnumerator.Current => this.Current;
//
//             public void Dispose() {
//                 this.Reset();
//             }
//         }
//
//         // disable code prompt
//         public struct ForeachEnumerator : IEnumerator<int> {
//             private BitList _list;
//
//             public int Current { get; private set; }
//
//             public ForeachEnumerator(BitList list) {
//                 this._list = list;
//                 this.Current = -1;
//             }
//
//             public bool MoveNext() {
//                 if (this._list.bits == null) return false;
//                 this.Current++;
//                 var bucketIndex = this.Current >> 6;
//                 var bitIndex = this.Current & 0x3f;
//                 for (int i = bucketIndex, len = this._list.bits.Length; i < len; i++, bitIndex = 0) {
//                     var bit = this._list.bits[i];
//                     if (bit == 0) continue;
//                     for (var j = bitIndex; j < BitOfElement; j++) {
//                         if ((bit & (1ul << j)) != 0) {
//                             this.Current = i * BitOfElement + j;
//                             return true;
//                         }
//                     }
//                 }
//
//                 return false;
//             }
//
//             public void Reset() {
//                 this._list = null;
//                 this.Current = -1;
//             }
//
//             object IEnumerator.Current => this.Current;
//
//             public void Dispose() { }
//         }
//     }
//
//     public static class BitListExtension {
//         public static bool IsNullOrEmpty(this BitList self) {
//             return BitList.IsNullOrEmpty(self);
//         }
//
//         public static void Add<T>(this BitList self, T enu) where T : Enum {
//             self.Add(enu.GetHashCode());
//         }
//
//         public static void Add<T>(this BitList self, IList<T> enums) where T : Enum {
//             for (int i = 0, len = enums.Count; i < len; i++) {
//                 self.Add(enums[i].GetHashCode());
//             }
//         }
//
//         public static void Remove<T>(this BitList self, T enu) where T : Enum {
//             self.Remove(enu.GetHashCode());
//         }
//
//         public static void Set<T>(this BitList self, IList<T> enums) where T : Enum {
//             self.Clear();
//             for (int i = 0, len = enums.Count; i < len; i++) {
//                 self.Add(enums[i].GetHashCode());
//             }
//         }
//
//         public static bool Contains<T>(this BitList self, T e) where T : Enum {
//             return self.Contains(e.GetHashCode());
//         }
//     }
// }