using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class NumericNode : Unbodied, INumericNode {
        [MemoryPackInclude]
        public string NodeName { get; set; }

        [MemoryPackInclude]
        public int NodeType { get; set; }

        // string: 一个Num的唯一标识. 应用场景: 例如有一个装备, 他有三个基础属性和1个特殊属性, 三个基础属性不会重复, 但那一个特殊属性却有可能会和三个基础属性重复, 所以需要加一个唯一标识
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        private MultiDictionary<uint, string, Num> _numerics = new(); // key1: numericType 与 numericLayer 与 numericModel的组合, key2: unique, value: 数值

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        private Dictionary<uint, Num> _finalNumerics = new(); // key: numericType 与 numericLayer 与 numericModel 三者的组合,  value: 数值

        [MemoryPackIgnore]
        public List<INumerator> linkNumerators = new();

        [MemoryPackIgnore]
        public IEnumerable<uint> Keys => this._numerics.Keys;

        [MemoryPackInclude]
        public NumericNodeLinkModel LinkModel { get; set; }

        protected override void OnDeserializedOverall() {
            this.Recalculate();
        }

        protected internal override void OnDisposed() {
            base.OnDisposed();
            this._numerics.Clear();
            this._finalNumerics.Clear();
            this.linkNumerators.Clear();
        }

        public bool LinkNumerator(INumerator numerator) {
            if (this.linkNumerators.Contains(numerator)) return false;
            this.linkNumerators.Add(numerator);
            return true;
        }

        public bool UnlinkNumerator(INumerator numerator) {
            return this.linkNumerators.Remove(numerator);
        }

        public void Foreach(Action<uint, string, Num> callback) {
            foreach (var kv in this._numerics) {
                foreach (var kvv in kv.Value) {
                    callback.Invoke(kv.Key, kvv.Key, kvv.Value);
                }
            }
        }

        public Num GetValue(NumericNodeKey key) {
            return this.GetValue(key.key);
        }

        private Num GetValue(uint key) {
            if (key < NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key cant be less than {NumericConst.NumericMaxTypeNumInTheory} '{key}'");
            return !this._finalNumerics.TryGetValue(key, out var result) ? Num.Empty() : result;
        }

        public Num GetValue(NumericNodeKey key, string unique) {
            return this.GetValue(key.key, unique);
        }

        private Num GetValue(uint key, string unique) {
            if (key < NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key cant be less than {NumericConst.NumericMaxTypeNumInTheory} '{key}'");
            return !this._numerics.TryGetValue(key, unique, out var result) ? Num.Empty() : result;
        }

        public void SetValue(NumericNodeKey key, Num value) {
            this.SetValue(key.key, value);
        }

        private void SetValue(uint key, Num value) {
            this.SetValue(key, value, "default");
        }

        public void SetValue(NumericNodeKey key, Num value, string unique) {
            this.SetValue(key.key, value, unique);
        }

        private void SetValue(uint key, Num value, string unique) {
            if (key < NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key cant be less than {NumericConst.NumericMaxTypeNumInTheory} '{key}'");

            if (!this._numerics.TryGetValue(key, unique, out var old)) {
                this._numerics[key, unique] = value;
                this.Recalculate(key);
                return;
            }

            if (old == value) return;

            this._numerics[key, unique] = value;
            this.Recalculate(key);
        }

        public void Recalculate() {
            foreach (var numKey in this._numerics.Keys) {
                this.Recalculate(numKey);
            }
        }

        private void Recalculate(uint key) {
            if (!this._numerics.TryGetValue(key, out var dict)) return;

            byte finalType = 64; // 64是随便取的, 只要比 Num._type的最大数大就行
            var finalValue = Num.Empty(finalType);
            foreach (var kv in dict) {
                if (finalType == 64) {
                    finalType = kv.Value.Type;
                }

                finalValue += kv.Value;
            }

            finalValue.Convert(finalType);
            this._finalNumerics.TryGetValue(key, out var old);
            this._finalNumerics[key] = finalValue;

            if (old != finalValue) {
                this.OnChanged(key);
            }
        }

        private void OnChanged(uint key) {
            if (this.linkNumerators == null) return;

            for (int i = 0, len = this.linkNumerators.Count; i < len; i++) {
                this.linkNumerators[i].Recalculate(key >> NumericConst.NumericTypeOffset);
            }
        }
    }
}