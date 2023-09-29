// using System;
// using System.Collections.Generic;
//
// namespace Hsenl {
//     [Serializable]
//     public class NumericNodeOld : Unbodied, INumericNode {
//         public string name;
//         private readonly Dictionary<uint, Num> _numerics = new(); // key: numericType 与 numericLayer 与 numericModel的组合,  value: 数值
//         
//         public List<INumerator> linkNumerators = new();
//
//         public IEnumerable<uint> Keys => this._numerics.Keys;
//
//         public void Link(INumerator numerator) {
//             this.linkNumerators.Add(numerator);
//         }
//
//         public void Unlink(INumerator numerator) {
//             this.linkNumerators.Remove(numerator);
//         }
//
//         public void SetValue(NumericNodeKey key, Num value) {
//             this.SetValue(key.key, value);
//         }
//
//         public Num GetValue(NumericNodeKey key) {
//             return this.GetValue(key.key);
//         }
//
//         public void SetValue(uint key, Num value) {
//             if (key < NumericConst.NumericTypeMax) throw new ArgumentOutOfRangeException($"num key cant be less than {NumericConst.NumericTypeMax} '{key}'");
//
//             if (!this._numerics.TryGetValue(key, out var old)) {
//                 this._numerics[key] = value;
//                 this.OnChanged(key);
//                 return;
//             }
//
//             if (old == value) return;
//
//             this._numerics[key] = value;
//             this.OnChanged(key);
//         }
//
//         public Num GetValue(uint key) {
//             if (key < NumericConst.NumericTypeMax) throw new ArgumentOutOfRangeException($"num key cant be less than {NumericConst.NumericTypeMax} '{key}'");
//             return !this._numerics.TryGetValue(key, out var result) ? Num.Empty() : result;
//         }
//
//         private void OnChanged(uint key) {
//             if (this.linkNumerators == null) return;
//
//             for (int i = 0, len = this.linkNumerators.Count; i < len; i++) {
//                 this.linkNumerators[i].Recalculate(key >> NumericConst.NumericTypeOffset);
//             }
//         }
//     }
// }