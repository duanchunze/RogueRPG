using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public abstract class ASelectionsSearcher : SelectionsObsolete {
        protected static readonly UnityEngine.Collider[] searchCaches = new UnityEngine.Collider[100];
        protected readonly List<SelectionTarget> searched = new(); // 搜寻后的目标

        public IReadOnlyList<SelectionTarget> Targets => this.searched;

        public abstract ASelectionsSearcher Search();

        public virtual ASelectionsSearcher Search(out List<SelectionTarget> outs) {
            this.Search();
            outs = this.searched;
            return this;
        }

        public ASelectionsFilter Filter(ASelectionsFilter filter) {
            if (this.isUpdate) filter.Obsolesce();
            filter.Filter(this.searched);
            return filter;
        }

        public void Select(ASelectionsSelector selector) {
            if (this.isUpdate) selector.Obsolesce();
            selector.Select(this.searched);
        }
    }
}