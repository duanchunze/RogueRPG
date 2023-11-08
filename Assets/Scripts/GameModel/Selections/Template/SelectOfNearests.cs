using System;
using System.Collections.Generic;
using Unity.Mathematics;

namespace Hsenl {
    [Serializable]
    public class SelectOfNearests : ASelectionsSelect {
        public float3 position;
        public int count;

        public override ASelectionsSelect Select(IReadOnlyList<SelectionTarget> sts) {
            this.selected.Clear();
            var array = new MergeSortFloatWrap<SelectionTarget>[sts.Count];
            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];
                var dis = math.distancesq(this.position, target.transform.Position);
                array[i] = new MergeSortFloatWrap<SelectionTarget>() { num = dis, value = target };
            }

            ArrayHelper.MergeSort(array);

            var arrayLength = array.Length;
            if (this.count == 0) Log.Error("SelectorsOfNearest count is 0, Have you forgotten?");
            var length = this.count > 0 ? this.count : arrayLength;
            for (var i = 0; i < length; i++) {
                if (i >= arrayLength) break;
                this.selected.Add(array[i]);
            }

            return this;
        }
    }
}