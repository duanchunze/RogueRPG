using System.Collections.Generic;

namespace Hsenl {
    public class FilterOfAlive : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];

                // 如果死亡, 则跳过
                if (Shortcut.IsDead(target.Bodied)) continue;

                this.filtered.Add(target);
            }

            return this;
        }
    }
}