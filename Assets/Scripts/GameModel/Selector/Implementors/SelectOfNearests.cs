using System;
using System.Buffers;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class SelectOfNearests : ASelectionsSelect {
        public Vector3 position;
        public int count;

        private ArrayPool<MergeSortFloatWrap<SelectionTarget>> _arrayPool = ArrayPool<MergeSortFloatWrap<SelectionTarget>>.Shared;

        public override ASelectionsSelect Select(IReadOnlyList<SelectionTarget> sts) {
            this.selected.Clear();

            var len = sts.Count;
            var array = this._arrayPool.Rent(len);
            var cache = this._arrayPool.Rent(len);

            try {
                for (int i = 0; i < len; i++) {
                    var target = sts[i];
                    var dis = Vector3.DistanceSquared(this.position, target.transform.Position);
                    array[i] = new MergeSortFloatWrap<SelectionTarget>() { num = dis, value = target };
                }

                ArrayHelper.MergeSort(array.AsSpan(0, len), cache.AsSpan(0, len));

                if (this.count == 0) Log.Error("SelectorsOfNearest count is 0, Have you forgotten?");
                var length = this.count > 0 ? this.count : len;
                for (var i = 0; i < length; i++) {
                    if (i >= len) break;
                    this.selected.Add(array[i]);
                }
            }
            finally {
                this._arrayPool.Return(array);
                this._arrayPool.Return(cache);
            }

            return this;
        }
    }
}