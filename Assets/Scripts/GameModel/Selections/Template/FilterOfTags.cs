using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class FilterOfTags : ASelectionsFilter {
        public IReadOnlyBitlist constrainsTags;
        public IReadOnlyBitlist exclusivesTags;

        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            var constrainsNull = this.constrainsTags.IsNullOrEmpty();
            var exclusivesNull = this.exclusivesTags.IsNullOrEmpty();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];

                if (!constrainsNull && !target.Tags.ContainsAny(this.constrainsTags)) continue;
                if (!exclusivesNull && target.Tags.ContainsAny(this.exclusivesTags)) continue;

                this.filtered.Add(target);
            }

            this.constrainsTags = null;
            this.exclusivesTags = null;

            return this;
        }
    }
}