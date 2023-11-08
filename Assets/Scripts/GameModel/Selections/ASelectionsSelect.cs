using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public abstract class ASelectionsSelect : ASelections {
        protected readonly List<SelectionTarget> selected = new(); // 选出的目标

        public override IReadOnlyList<SelectionTarget> Targets => this.selected;

        public abstract ASelectionsSelect Select(IReadOnlyList<SelectionTarget> sts);
        
        public override ASelectionsFilter Filter(ASelectionsFilter filter) {
            filter.Filter(this.selected);
            return filter;
        }

        public override ASelectionsSelect Select(ASelectionsSelect select) {
            select.Select(this.selected);
            return select;
        }
    }
}