using System.Collections.Generic;

namespace Hsenl {
    // 过滤出背对自己的目标
    public class FilterOfBackToSelf : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];

                if (!Shortcut.IsBackForTarget(target.transform, this.Selector.transform)) {
                    continue;
                }

                this.filtered.Add(target);
            }

            return this;
        }
    }
}