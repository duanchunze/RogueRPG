using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class FilterOfTags : ASelectionsFilter {
        public IReadOnlyBitlist constrainsTags;
        public IReadOnlyBitlist exclusivesTags;

        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> ins) {
            if (!this.IsObsolete) {
                return this;
            }

            this.filtered.Clear();

            var constrainsNull = this.constrainsTags.IsNullOrEmpty();
            var exclusivesNull = this.exclusivesTags.IsNullOrEmpty();

            for (int i = 0, len = ins.Count; i < len; i++) {
                var target = ins[i];

                if (!constrainsNull && !target.ContainsAll(this.constrainsTags)) continue;
                if (!exclusivesNull && target.ContainsAny(this.exclusivesTags)) continue;

                this.filtered.Add(target);
            }

            this.constrainsTags = null;
            this.exclusivesTags = null;

            this.RecalculateObsoleteFrame();

            return this;
        }
    }
}