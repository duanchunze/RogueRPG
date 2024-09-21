using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class FilterOfTargets : ASelectionsFilter {
        public List<Bodied> targets;

        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];
                if (this.targets.Contains(target.Bodied))
                    continue;

                this.filtered.Add(target);
            }

            return this;
        }
    }
}