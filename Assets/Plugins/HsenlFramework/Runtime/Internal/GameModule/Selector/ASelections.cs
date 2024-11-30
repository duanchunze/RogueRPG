using System.Collections.Generic;

namespace Hsenl {
    public abstract class ASelections {
        public Selector Selector { get; internal set; }

        public abstract IReadOnlyList<SelectionTarget> Targets { get; }

        public SelectionTarget Target => this.Targets.Count != 0 ? this.Targets[0] : null;

        public abstract ASelectionsFilter Filter(ASelectionsFilter filter);

        public abstract ASelectionsSelect Select(ASelectionsSelect select);

        // 打包袋
        public void Wrap(List<SelectionTarget> cache) {
            for (int i = 0, len = this.Targets.Count; i < len; i++) {
                var target = this.Targets[i];
                cache.Add(target);
            }
        }

        public void WrapAs<T>(List<T> cache) where T : SelectionTarget {
            for (int i = 0, len = this.Targets.Count; i < len; i++) {
                var target = this.Targets[i];
                cache.Add((T)target);
            }
        }
    }
}