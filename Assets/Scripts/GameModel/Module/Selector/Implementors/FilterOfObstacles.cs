using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class FilterOfObstacles : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];

                if (Physics.Linecast(this.selector.transform.Position, target.transform.Position, 1 << 3)) {
                    continue;
                }

                this.filtered.Add(target);
            }

            return this;
        }
    }
}