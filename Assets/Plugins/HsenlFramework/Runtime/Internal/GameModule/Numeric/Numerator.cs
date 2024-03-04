using System;
using System.Collections.Generic;
using System.Linq;
using Hsenl.EventType;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    /* 整个数值分为两块
     * Numerator and Node
     * Numerator: 是对结果进行一个汇总的地方
     * Node: 影响汇总结果
     * NumericKey: 对应着NumericType
     * NodeLayerKey: 决定该node会在第几层被纳入计算
     * NodeTypeKey: 决定该node的计算方式
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
        public static uint TotalLayerNum { get; private set; }

        public static void InitNumerator(int totalLayerNum) {
            TotalLayerNum = (uint)totalLayerNum;
            if (TotalLayerNum is < 1 or > NumericConst.NodeLayerMaxInTheory) {
                throw new ArgumentOutOfRangeException($"total layer num must be range of 1 - {NumericConst.NodeLayerMaxInTheory} '{TotalLayerNum}'");
            }
        }

        public static void InitNumerator<T>(T totalLayerNum) where T : Enum {
            TotalLayerNum = (uint)totalLayerNum.GetHashCode();
            if (TotalLayerNum is < 1 or > NumericConst.NodeLayerMaxInTheory) {
                throw new ArgumentOutOfRangeException($"total layer num must be range of 1 - {NumericConst.NodeLayerMaxInTheory} '{TotalLayerNum}'");
            }
        }

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

        // final里存的不仅仅只是最终数值, 还有每一层的最终数值
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private MultiDictionary<uint, uint, Num> _finalNumerics = new(); // key1: NumericType key2: NumericLayer value: 最终值

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private readonly List<INumericNode> _attaches = new();

        [MemoryPackIgnore]
        public INumericNode[] Attaches => this._attaches.ToArray();

        public event Action<Numerator, int, Num, Num> OnNumericChanged; // numerator, numType, old, now

        protected override void OnDeserializedOverall() {
            this.Recalculate();
        }

        protected internal override void OnDestroyFinish() {
            base.OnDestroyFinish();
            this.allowAttacherExpand = false;
            this._rawNumerics.Clear();
            this._attachNumerics.Clear();
            this._finalNumerics.Clear();
            this._attaches.Clear();
        }

        protected override void OnAwake() {
            if (TotalLayerNum == 0)
                throw new Exception("numerator is not initialized");
        }

        public bool Attach(INumericNode node) {
            if (this._attaches.Contains(node)) return false;
            this._attaches.Add(node);
            foreach (var key in node.Keys) {
                var numericType = key >> NumericConst.NumericTypeOffset;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType);
            }

            return true;
        }

        public bool Detach(INumericNode node) {
            var success = this._attaches.Remove(node);
            if (!success) return false;
            foreach (var key in node.Keys) {
                var numericType = key >> NumericConst.NumericTypeOffset;
                this.RecalculateAttachNumeric(numericType);
                this.RecalculateFinalNumeric(numericType);
            }

            return true;
        }

        public bool IsHasValue(uint numericType) {
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");

            return this._rawNumerics.ContainsKey(numericType);
        }

        // 获取未处理的值, 是最最原始的数值, 一般来说, 就是人物的初始属性, 之所以说是最原始的, 因为可能有些词条指名增加基础属性, 虽然该数值会被算在基础数值上, 但却不是人物的初始数值
        public Num GetRawValue(uint numericType) {
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");
            return !this._rawNumerics.TryGetValue(numericType, out var result) ? Num.Empty() : result;
        }

        // 设置原始值, 一般来说, 我们如果给Numerator设置值, 那设置的就是这个值
        public void SetRawValue(uint numericType, Num value, bool sendEvent = true) {
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");

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
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory) {
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");
            }

            if (layer is < 1 or > NumericConst.NodeLayerMaxInTheory) {
                throw new ArgumentOutOfRangeException($"layer num must be range of 1 - {NumericConst.NodeLayerMaxInTheory} '{layer}'");
            }

            var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
            var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);

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
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory)
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");
            return !this._finalNumerics.TryGetValue(numericType, TotalLayerNum - 1, out var result) ? Num.Empty() : result;
        }

        public Num GetFinalValue(uint numericType, uint layer) {
            if (numericType is < 1 or > NumericConst.NumericTypeMaxInTheory) {
                throw new ArgumentOutOfRangeException($"num key must be range of 1 - {NumericConst.NumericTypeMaxInTheory} '{numericType}'");
            }

            if (layer is < 1 or > NumericConst.NodeLayerMaxInTheory) {
                throw new ArgumentOutOfRangeException($"layer num must be range of 1 - {NumericConst.NodeLayerMaxInTheory} '{layer}'");
            }

            return !this._finalNumerics.TryGetValue(numericType, layer, out var result) ? Num.Empty() : result;
        }

        private void RecalculateAttachNumeric(uint numericType) {
            for (uint layer = 1; layer < TotalLayerNum; layer++) {
                var pct = new Num(1f); // percentage of node value in current layer
                var fix = new Num(); // fixed of node value in current layer
                var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
                var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);

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

            this._finalNumerics.TryGetValue(numericType, TotalLayerNum - 1, out var old);
            for (uint layer = 1; layer < TotalLayerNum; layer++) {
                var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
                var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);

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
                        this._finalNumerics[numericType, layer] = finalValue;
                    }
                }
                else {
                    finalValue = finalValue * pct + fix; // 先按比例乘, 再加上固定值
                    finalValue.Convert(finalType);
                    this._finalNumerics[numericType, layer] = finalValue;
                }
            }

            // 如果该值为0, 且基础值里根本没有该类型的值, 说明该值是Node添加进来的, 则删除
            if (finalValue == 0 && !basicHas) {
                this._finalNumerics.Remove(numericType);
            }

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
            Span<uint> array1 = stackalloc uint[this._attachNumerics.Count];
            var n1 = 0;
            foreach (var key in this._attachNumerics.Keys) {
                array1[n1++] = key >> NumericConst.NumericTypeOffset;
            }

            for (int i = 0, len = array1.Length; i < len; i++) {
                this.RecalculateAttachNumeric(array1[i]);
            }

            Span<uint> array2 = stackalloc uint[this._attachNumerics.Count];
            var n2 = 0;
            foreach (var finalNumericsKey in this._finalNumerics.Keys) {
                array2[n2++] = finalNumericsKey;
            }

            for (int i = 0, len = array2.Length; i < len; i++) {
                this.RecalculateFinalNumeric(array2[i]);
            }
        }

        public void Recalculate(uint numericType, bool sendEvent = true) {
            this.RecalculateAttachNumeric(numericType);
            this.RecalculateFinalNumeric(numericType, sendEvent);
        }

        // 提供该函数的原因在于, 如果是Recalculate的话, 会影响finalNumeric里的数据
        // 而该函数只做一次临时计算, 并不会把数据缓存到finalNumeric里
        public Num CalculateValue(NumericNode node, uint numericType, bool allowNodeExpend = false) {
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

            for (uint layer = 1; layer < TotalLayerNum; layer++) {
                var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
                var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);
                if (!this._attachNumerics.TryGetValue(pctKey.key, out var pct)) {
                    pct = new Num(1f);
                }

                if (!this._attachNumerics.TryGetValue(fixKey.key, out var fix)) {
                    fix = new Num();
                }

                pct += node.GetValue(pctKey);
                fix += node.GetValue(fixKey);

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

        // 把两个numerator的基础值相加, 然后把各自的numeric node合并到一起, 计算出最终结果
        public Num MergeCalculateValue(Numerator numerator, uint numericType, bool allowNodeExpend = false) {
            byte finalType = 64; // 64是随便取的, 只要比Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();

            var n = 0;
            if (!this._rawNumerics.TryGetValue(numericType, out var basicValue1)) {
                n++;
            }

            if (!numerator._rawNumerics.TryGetValue(numericType, out var basicValue2)) {
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

            for (uint layer = 1; layer < TotalLayerNum; layer++) {
                var pct = new Num(1f);
                var fix = new Num();
                var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
                var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);
                for (int i = 0, len = this._attaches.Count; i < len; i++) {
                    var attacher = this._attaches[i];
                    pct += attacher.GetValue(pctKey);
                    fix += attacher.GetValue(fixKey);
                }

                for (int i = 0, len = numerator._attaches.Count; i < len; i++) {
                    var attacher = numerator._attaches[i];
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

        public static Num MergeCalculateValue(IList<Numerator> numerators, uint numericType, bool allowNodeExpen = false) {
            byte finalType = 64; // 64是随便取的, 只要比Num._type的最大数大就行, 因为byte不能为负, 所以无法使用-1标记该字段未设置.
            var finalValue = Num.Empty();

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

            for (uint layer = 1; layer < TotalLayerNum; layer++) {
                var pct = new Num(1f);
                var fix = new Num();
                var pctKey = new NumericNodeKey(numericType, layer, NumericNodeModel.Percentage);
                var fixKey = new NumericNodeKey(numericType, layer, NumericNodeModel.FixedValue);

                for (int i = 0, len = numerators.Count; i < len; i++) {
                    var numerator = numerators[i];
                    for (var j = 0; j < numerator._attaches.Count; j++) {
                        var attacher = numerator._attaches[j];
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

        public Num GetFinalValue<T>(T numericType, uint layer) where T : Enum {
            return this.GetFinalValue((uint)numericType.GetHashCode(), layer);
        }

        public void Recalculate<T>(T numericType, bool sendEvent = true) where T : Enum {
            this.Recalculate((uint)numericType.GetHashCode(), sendEvent);
        }

        public Num CalculateValue<T>(NumericNode node, T numericType, bool allowNodeExpend = false) where T : Enum {
            return CalculateValue(node, (uint)numericType.GetHashCode(), allowNodeExpend);
        }

        // 把两个numerator的基础值相加, 然后把各自的numeric node合并到一起, 计算出最终结果
        public Num MergeCalculateValue<T>(Numerator numerator, T numericType, bool allowNodeExpend = false) where T : Enum {
            return MergeCalculateValue(numerator, (uint)numericType.GetHashCode(), allowNodeExpend);
        }

        public static Num MergeCalculateValue<T>(IList<Numerator> numerators, T numericType, bool allowNodeExpen = false) where T : Enum {
            return MergeCalculateValue(numerators, (uint)numericType.GetHashCode(), allowNodeExpen);
        }
    }
}