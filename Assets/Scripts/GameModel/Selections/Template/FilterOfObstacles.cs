using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class FilterOfObstacles : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> ins) {
            if (!this.IsObsolete) {
                return this;
            }

            this.filtered.Clear();

            for (int i = 0, len = ins.Count; i < len; i++) {
                var target = ins[i];

                if (Physics.Linecast(this.selector.transform.Position, target.transform.Position, 1 << 3)) {
                    continue;
                }

                this.filtered.Add(target);
            }

            this.RecalculateObsoleteFrame();

            return this;
        }
    }
}