using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Hsenl {
    [Serializable]
    public class SelectorsOfNearest : ASelectionsSelector {
        public float3 position;
        public int count;

        // private SortedList<float, SelectionTarget> cache = new();
        private List<SelectionTarget> selectors = new();

        public IReadOnlyList<SelectionTarget> Targets => this.selectors;

        // public override ASelectionsSelector Select(IReadOnlyList<SelectionTarget> ins) {
        //     if (!this.IsObsolete) {
        //         return this;
        //     }
        //
        //     this.cache.Clear();
        //     for (int i = 0, len = ins.Count; i < len; i++) {
        //         var target = ins[i];
        //         var dis = math.distancesq(this.position, target.transform.position);
        //         this.cache.Add(dis, target);
        //     }
        //
        //     this.selectors.Clear();
        //     var c = 0;
        //     foreach (var target in this.cache) {
        //         if (c >= this.count) break;
        //         this.selectors.Add(target.Value);
        //         c++;
        //     }
        //
        //     if (this.selectors.Count != 0) {
        //         this.selected = this.selectors[0];
        //     }
        //
        //     this.RecalculateObsoleteFrame();
        //
        //     return this;
        // }

        public override ASelectionsSelector Select(IReadOnlyList<SelectionTarget> ins) {
            if (!this.IsObsolete) {
                return this;
            }

            var array = new MergeSortFloatWrap<SelectionTarget>[ins.Count];
            for (int i = 0, len = ins.Count; i < len; i++) {
                var target = ins[i];
                var dis = math.distancesq(this.position, target.transform.Position);
                array[i] = new MergeSortFloatWrap<SelectionTarget>() { num = dis, value = target };
            }

            ArrayHelper.MergeSort(array);

            this.selectors.Clear();

            var arrayLength = array.Length;
            if (this.count == 0) Log.Warning("SelectorsOfNearest count is 0, Have you forgotten?");
            var length = this.count >= 0 ? this.count : arrayLength;
            for (var i = 0; i < length; i++) {
                if (i >= arrayLength) break;
                this.selectors.Add(array[i]);
            }

            if (this.selectors.Count != 0) {
                this.selected = this.selectors[0];
            }

            this.RecalculateObsoleteFrame();
            return this;
        }
    }
}