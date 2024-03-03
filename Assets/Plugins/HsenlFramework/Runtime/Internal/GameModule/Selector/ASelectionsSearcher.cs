using System.Collections.Generic;

namespace Hsenl {
    public abstract class ASelectionsSearcher : ASelections {
        protected readonly List<SelectionTarget> searched = new(); // 搜寻后的目标

        public override IReadOnlyList<SelectionTarget> Targets => this.searched;

        public abstract ASelectionsSearcher Search();

        public override ASelectionsFilter Filter(ASelectionsFilter filter) {
            filter.Filter(this.searched);
            return filter;
        }

        public override ASelectionsSelect Select(ASelectionsSelect select) {
            select.Select(this.searched);
            return select;
        }
    }
}