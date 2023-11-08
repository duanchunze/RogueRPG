using System.Collections.Generic;
using Hsenl.cast;

namespace Hsenl {
    public abstract class ASelections {
        public Selector selector;

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
    }
}