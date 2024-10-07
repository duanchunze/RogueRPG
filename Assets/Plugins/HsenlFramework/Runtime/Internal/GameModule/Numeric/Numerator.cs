using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Hsenl.EventType;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    /* 整个数值分为两块
     * Numerator and Numeric
     * Numerator: 是对结果进行一个汇总的地方
     * Numeric: 一个数值节点
     *
     * 算法: 每个节点是如何影响结果的, 具体算法怎么算, 算法顺序
     *      算法就是, 拿到基础值, 然后遍历第一层, 把固定数先加一起算出结果 a, 把百分率加一起算一个结果 b, 用基础值*百分率算出一个结果c, 最终结果 = 基础值 + a + c
     *      然后再遍历第二层, 使用第一层算出的结果作为基础值
     */

    namespace EventType {
        public struct OnNumericChanged {
            public Numerator numerator;
            public int numType;
            public Num old;
            public Num now;
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class Numerator : Unbodied, INumerator {
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public uint MaxLayer { get; private set; }

        // 默认如果Numerator中不包含某个数值的话, 那么即便Node里有, 也没用. 除非该值为true的.
        [MemoryPackInclude]
        public bool allowAttacherExpand;

        // 原始数值的意义在于它指定了该数值的最终numType, 此后所有的附加数值, 都最终会转化成该numType. 所以即使原始值为0, 我们也应该设置它, 而不是忽略它
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        private Dictionary<uint, Num> _rawNumerics = new(); // key: NumericType  value: 原始值

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private Dictionary<uint, Num> _attachNumerics = new(); // key: numericType 与 numericLayer 与 numericModel的组合, 也就是NumericNodeKey

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private Dictionary<uint, Num> _finalNumerics = new(); // key: NumericType  value: 最终值

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private readonly List<INumeric> _attaches = new();


        [MemoryPackIgnore]
        public INumeric[] Attaches => this._attaches.ToArray();

        public event Action<Numerator, int, Num, Num> OnNumericChanged; // numerator, numType, old, now

        protected override void OnDeserialized() {
            foreach (var kv in this._rawNumerics) {
                this.RecalculateFinalNumeric(kv.Key);
            }
        }

        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            foreach (var node in this._attaches) {
                node.UnlinkNumerator(this);
            }

            this.MaxLayer = 0;
            this.allowAttacherExpand = false;
            this._rawNumerics.Clear();
            this._attachNumerics.Clear();
            this._finalNumerics.Clear();
            this._attaches.Clear();
        }

        public bool Attach(INumeric numeric) {
            if (this._attaches.Contains(numeric)) return false;
            this._attaches.Add(numeric);
            if (numeric.MaxLayer > this.MaxLayer)
                this.MaxLayer = numeric.MaxLayer;

            foreach (var key in numeric.Keys) {
                var numericType = key >> NumericConst.NumericTypeOffset;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType);
            }

            numeric.LinkNumerator(this);
            return true;
        }

        public bool Detach(INumeric numeric) {
            var success = this._attaches.Remove(numeric);
            if (!success) return false;
            this.RecalculateMaxLayer();
            foreach (var key in numeric.Keys) {
                var numericType = key >> NumericConst.NumericTypeOffset;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType);
            }

            numeric.UnlinkNumerator(this);
            return true;
        }

        public bool IsHasValue(uint numericType) {
            if (numericType is < 1 or > NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericMaxTypeNumInTheory} '{numericType}'");

            return this._rawNumerics.ContainsKey(numericType);
        }

        // 获取未处理的值, 是最最原始的数值, 一般来说, 就是人物的初始属性, 之所以说是最原始的, 因为可能有些词条指名增加基础属性, 虽然该数值会被算在基础数值上, 但却不是人物的初始数值
        public Num GetRawValue(uint numericType) {
            if (numericType is < 1 or > NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericMaxTypeNumInTheory} '{numericType}'");
            return !this._rawNumerics.TryGetValue(numericType, out var result) ? Num.Empty() : result;
        }

        // 设置原始值, 一般来说, 我们如果给Numerator设置值, 那设置的就是这个值
        public void SetRawValue(uint numericType, Num value, bool sendEvent = true) {
            if (numericType is < 1 or > NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericMaxTypeNumInTheory} '{numericType}'");

            if (!this._rawNumerics.TryGetValue(numericType, out var old)) {
                this._rawNumerics[numericType] = value;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType, sendEvent);
                return;
            }

            if (old != value) {
                this._rawNumerics[numericType] = value;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType, sendEvent);
                return;
            }

            if (sendEvent) {
                EventSystem.Publish(new OnNumericChanged() { numerator = this, old = old, now = value, numType = (int)numericType });
            }
        }

        public (Num fix, Num pct) GetAttachValue(uint numericType, uint layer) {
            if (numericType is < 1 or > NumericConst.NumericMaxTypeNumInTheory) {
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericMaxTypeNumInTheory} '{numericType}'");
            }

            if (layer is < 1 or > NumericConst.NumericMaxLayerNumInTheory) {
                throw new ArgumentOutOfRangeException($"layer num must be range of 1 - {NumericConst.NumericMaxLayerNumInTheory} '{layer}'");
            }

            var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
            var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);

            if (!this._attachNumerics.TryGetValue(pctKey.key, out var pct)) {
                pct = new Num(1f);
            }

            if (!this._attachNumerics.TryGetValue(fixKey.key, out var fix)) {
                fix = new Num();
            }

            return (fix, pct);
        }

        // 获取最终值, 一般来说, 我们用的就是该值
        public Num GetFinalValue(uint numericType) {
            if (numericType is < 1 or > NumericConst.NumericMaxTypeNumInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericMaxTypeNumInTheory} '{numericType}'");
            return !this._finalNumerics.TryGetValue(numericType, out var result) ? Num.Empty() : result;
        }

        private void RecalculateAttachNumeric(uint numericType) {
            if (this._attaches.Count == 0)
                return;

            for (uint layer = 1; layer <= this.MaxLayer; layer++) {
                var pct = new Num(1f); // percentage of node value in current layer
                var fix = new Num(); // fixed of node value in current layer
                var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
                var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);

                foreach (var attacher in this._attaches) {
                    pct += attacher.GetValue(pctKey);
                    fix += attacher.GetValue(fixKey);
                }

                if (pct != 1f) {
                    this._attachNumerics[pctKey.key] = pct;
                }
                else {
                    this._attachNumerics.Remove(pctKey.key);
                }

                if (fix != 0) {
                    this._attachNumerics[fixKey.key] = fix;
                }
                else {
                    this._attachNumerics.Remove(fixKey.key);
                }
            }
        }

        private void RecalculateFinalNumeric(uint numericType, bool sendEvent = true) {
            var basicHas = false;
            byte finalType = 64; // 64是随便取的, 只要比 Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();
            if (!this._rawNumerics.TryGetValue(numericType, out var basicValue)) {
                if (!this.allowAttacherExpand) {
                    return;
                }
            }
            else {
                basicHas = true;
                finalType = basicValue.Type; // basic value 决定了该数值的最终 numType
                finalValue = basicValue;
            }

            this._finalNumerics.TryGetValue(numericType, out var old);
            for (uint layer = 1; layer <= this.MaxLayer; layer++) {
                var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
                var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);

                byte n = 0;
                if (!this._attachNumerics.TryGetValue(pctKey.key, out var pct)) {
                    pct = new Num(1f);
                    n++;
                }

                if (!this._attachNumerics.TryGetValue(fixKey.key, out var fix)) {
                    fix = new Num();
                    n++;
                }
                else {
                    if (finalType == 64) {
                        finalType = fix.Type;
                    }
                }

                if (n == 2) {
                    if (basicHas) {
                        // this._finalNumerics[numericType, layer] = finalValue;
                    }
                }
                else {
                    finalValue = finalValue * pct + fix; // 先按比例乘, 再加上固定值
                    finalValue.Convert(finalType);
                    // this._finalNumerics[numericType, layer] = finalValue;
                }
            }

            // // 如果该值为0, 且基础值里根本没有该类型的值, 则不添加
            // if (finalValue == 0 && !basicHas) {
            //     return;
            // }

            if (finalValue.Type == 64)
                finalValue.Convert(1);

            this._finalNumerics[numericType] = finalValue;

            if (sendEvent) {
                try {
                    this.OnNumericChanged?.Invoke(this, (int)numericType, old, finalValue);
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                EventSystem.Publish(new OnNumericChanged() { numerator = this, numType = (int)numericType, old = old, now = finalValue });
            }
        }

        public void Recalculate(bool sendEvent = true) {
            using var list = ListComponent<uint>.Rent();
            foreach (var kv in this._rawNumerics) {
                list.Add(kv.Key);
            }

            foreach (var attach in this._attaches) {
                foreach (var key in attach.Keys) {
                    if (!list.Contains(key))
                        list.Add(key);
                }
            }

            for (int i = 0, len = list.Count; i < len; i++) {
                this.RecalculateAttachNumeric(list[i]);
                this.RecalculateFinalNumeric(list[i]);
            }
        }

        public void Recalculate(uint numericType, bool sendEvent = true) {
            this.RecalculateAttachNumeric(numericType);
            this.RecalculateFinalNumeric(numericType, sendEvent);
        }

        public void RecalculateMaxLayer() {
            this.MaxLayer = 0;
            foreach (var numeric in this._attaches) {
                if (numeric.MaxLayer > this.MaxLayer)
                    this.MaxLayer = numeric.MaxLayer;
            }
        }

        public void ClearRaw() {
            this._rawNumerics.Clear();
        }

        public uint[] GetAllRawNumericTypes() {
            return this._rawNumerics.Keys.ToArray();
        }

        public uint[] GetAllAttachNumericKeys() {
            return this._attachNumerics.Keys.ToArray();
        }

        public uint[] GetAllFinalNumericTypes() {
            return this._finalNumerics.Keys.ToArray();
        }


        // 提供该函数的原因在于, 如果是Recalculate的话, 会影响finalNumeric里的数据
        // 而该函数只做一次临时计算, 并不会把数据缓存到finalNumeric里
        public Num CalculateValue(Numeric numeric, uint numericType, bool allowNodeExpend = false) {
            byte finalType = 64; // 64是随便取的, 只要比Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();
            if (!this._rawNumerics.TryGetValue(numericType, out var basicValue)) {
                if (!allowNodeExpend) {
                    return Num.Empty();
                }
            }
            else {
                finalType = basicValue.Type; // basic value 决定了该数值的最终 numType
                finalValue = basicValue;
            }

            for (uint layer = 1; layer <= this.MaxLayer; layer++) {
                var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
                var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);
                if (!this._attachNumerics.TryGetValue(pctKey.key, out var pct)) {
                    pct = new Num(1f);
                }

                if (!this._attachNumerics.TryGetValue(fixKey.key, out var fix)) {
                    fix = new Num();
                }

                pct += numeric.GetValue(pctKey);
                fix += numeric.GetValue(fixKey);

                if (finalType == 64) {
                    if (fix != 0) {
                        finalType = fix.Type;
                    }
                }

                finalValue = finalValue * pct + fix; // 先按比例乘, 再加上固定值
            }

            finalValue.Convert(finalType);

            return finalValue;
        }

        // 把两个numerator的基础值相加, 然后把各自的numeric node合并到一起, 计算出最终结果, 最终结果不会缓存在任何numerator之中
        public Num MergeCalculateValue(Numerator other, uint numericType, bool allowNodeExpend = false) {
            byte finalType = 64; // 64是随便取的, 只要比Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();
            var maxlayer = Math.Max(this.MaxLayer, other.MaxLayer);

            var n = 0;
            if (!this._rawNumerics.TryGetValue(numericType, out var basicValue1)) {
                n++;
            }

            if (!other._rawNumerics.TryGetValue(numericType, out var basicValue2)) {
                n++;
            }

            if (n == 2) {
                if (!allowNodeExpend) {
                    return Num.Empty();
                }
            }
            else {
                var basicValue = basicValue1 + basicValue2;
                finalType = basicValue.Type; // basic value 决定了该数值的最终 numType
                finalValue = basicValue;
            }

            for (uint layer = 1; layer <= maxlayer; layer++) {
                var pct = new Num(1f);
                var fix = new Num();
                var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
                var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);
                for (int i = 0, len = this._attaches.Count; i < len; i++) {
                    var attacher = this._attaches[i];
                    if (layer > attacher.MaxLayer)
                        continue;
                    pct += attacher.GetValue(pctKey);
                    fix += attacher.GetValue(fixKey);
                }

                for (int i = 0, len = other._attaches.Count; i < len; i++) {
                    var attacher = other._attaches[i];
                    if (layer > attacher.MaxLayer)
                        continue;
                    pct += attacher.GetValue(pctKey);
                    fix += attacher.GetValue(fixKey);
                }

                if (finalType == 64) {
                    if (fix != 0) {
                        finalType = fix.Type;
                    }
                }

                finalValue = finalValue * pct + fix; // 先按比例乘, 再加上固定值
            }

            finalValue.Convert(finalType);

            return finalValue;
        }

        // 同上, 但是是合并若干个
        public static Num MergeCalculateValue(IList<Numerator> numerators, uint numericType, bool allowNodeExpen = false) {
            byte finalType = 64; // 64是随便取的, 只要比Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();
            uint maxlayer = 0;
            for (int i = 0; i < numerators.Count; i++) {
                var numerator = numerators[i];
                if (numerator.MaxLayer > maxlayer)
                    maxlayer = numerator.MaxLayer;
            }

            var n = 0;
            for (int i = 0, len = numerators.Count; i < len; i++) {
                var numerator = numerators[i];
                if (!numerator._rawNumerics.TryGetValue(numericType, out var basicValue)) {
                    n++;
                }
                else {
                    finalValue += basicValue;
                }
            }

            if (n == numerators.Count) {
                if (!allowNodeExpen) {
                    return Num.Empty();
                }
            }
            else {
                finalType = finalValue.Type;
            }

            for (uint layer = 1; layer <= maxlayer; layer++) {
                var pct = new Num(1f);
                var fix = new Num();
                var pctKey = new NumericKey(numericType, layer, NumericMode.Percentage);
                var fixKey = new NumericKey(numericType, layer, NumericMode.FixedValue);

                for (int i = 0, len = numerators.Count; i < len; i++) {
                    var numerator = numerators[i];
                    if (layer > numerator.MaxLayer)
                        continue;
                    for (var j = 0; j < numerator._attaches.Count; j++) {
                        var attacher = numerator._attaches[j];
                        if (layer > attacher.MaxLayer)
                            continue;
                        pct += attacher.GetValue(pctKey);
                        fix += attacher.GetValue(fixKey);
                    }
                }

                if (finalType == 64) {
                    if (fix != 0) {
                        finalType = fix.Type;
                    }
                }

                finalValue = finalValue * pct + fix; // 先按比例乘, 再加上固定值
            }

            finalValue.Convert(finalType);

            return finalValue;
        }

        public bool IsHasValue<T>(T numericType) where T : Enum {
            return this.IsHasValue((uint)numericType.GetHashCode());
        }

        public Num GetRawValue<T>(T numericType) where T : Enum {
            return this.GetRawValue((uint)numericType.GetHashCode());
        }

        public void SetRawValue<T>(T numericType, Num value, bool sendEvent = true) where T : Enum {
            this.SetRawValue((uint)numericType.GetHashCode(), value, sendEvent);
        }

        public (Num fix, Num pct) GetAttachValue<T>(T numericType, uint layer) where T : Enum {
            return this.GetAttachValue((uint)numericType.GetHashCode(), layer);
        }

        public Num GetFinalValue<T>(T numericType) where T : Enum {
            return this.GetFinalValue((uint)numericType.GetHashCode());
        }

        public void Recalculate<T>(T numericType, bool sendEvent = true) where T : Enum {
            this.Recalculate((uint)numericType.GetHashCode(), sendEvent);
        }

        // 单独计算附加某个Node后的结果, 结果不会缓存在numerator之中
        public Num CalculateValue<T>(Numeric numeric, T numericType, bool allowNodeExpend = false) where T : Enum {
            return this.CalculateValue(numeric, (uint)numericType.GetHashCode(), allowNodeExpend);
        }

        // 把两个numerator的基础值相加, 然后把各自的numeric node合并到一起, 计算出最终结果, 最终结果不会缓存在任何numerator之中
        public Num MergeCalculateValue<T>(Numerator other, T numericType, bool allowNodeExpend = false) where T : Enum {
            return this.MergeCalculateValue(other, (uint)numericType.GetHashCode(), allowNodeExpend);
        }

        // 同上, 但是是合并若干个
        public static Num MergeCalculateValue<T>(IList<Numerator> numerators, T numericType, bool allowNodeExpen = false) where T : Enum {
            return MergeCalculateValue(numerators, (uint)numericType.GetHashCode(), allowNodeExpen);
        }
    }
}