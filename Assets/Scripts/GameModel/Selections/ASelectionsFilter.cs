using System.Collections.Generic;

namespace Hsenl {
    public abstract class ASelectionsFilter : SelectionsObsolete {
        protected readonly List<SelectionTarget> filtered = new(); // 过滤后的目标

        public IReadOnlyList<SelectionTarget> Targets => this.filtered;

        public abstract ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> ins);

        public virtual ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> ins, out List<SelectionTarget> outs) {
            this.Filter(ins);
            outs = this.filtered;
            return this;
        }
        
        public ASelectionsFilter Filter(ASelectionsFilter filter) {
            if (this.isUpdate) filter.Obsolesce();
            filter.Filter(this.filtered);
            return filter;
        }
        
        public void Select(ASelectionsSelector selector) {
            if (this.isUpdate) selector.Obsolesce();
            selector.Select(this.filtered);
        }
    }
}