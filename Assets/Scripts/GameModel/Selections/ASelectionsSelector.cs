using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public abstract class ASelectionsSelector : SelectionsObsolete {
        protected SelectionTarget selected; // 选择后的目标

        public SelectionTarget Target => this.selected;
        
        public abstract ASelectionsSelector Select(IReadOnlyList<SelectionTarget> ins);

        public virtual ASelectionsSelector Select(IReadOnlyList<SelectionTarget> ins, out SelectionTarget target) {
            this.Select(ins);
            target = this.selected;
            return this;
        }
    }
}