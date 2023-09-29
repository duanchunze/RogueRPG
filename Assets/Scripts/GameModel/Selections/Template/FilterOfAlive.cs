using System.Collections.Generic;

namespace Hsenl {
    public class FilterOfAlive : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> ins) {
            if (!this.IsObsolete) {
                return this;
            }

            this.filtered.Clear();

            for (int i = 0, len = ins.Count; i < len; i++) {
                var target = ins[i];

                // 如果死亡, 则跳过
                if (Shortcut.IsDead(target.Substantive)) continue;

                this.filtered.Add(target);
            }

            this.RecalculateObsoleteFrame();
            return this;
        }
    }
}