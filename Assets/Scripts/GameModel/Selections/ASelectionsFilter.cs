using System.Collections.Generic;

namespace Hsenl {
    public abstract class ASelectionsFilter : ASelections {
        protected readonly List<SelectionTarget> filtered = new(); // 过滤后的目标

        public override IReadOnlyList<SelectionTarget> Targets => this.filtered;

        public abstract ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts);

        public override ASelectionsFilter Filter(ASelectionsFilter filter) {
            filter.Filter(this.filtered);
            return filter;
        }

        public override ASelectionsSelect Select(ASelectionsSelect select) {
            select.Select(this.filtered);
            return select;
        }
    }
}