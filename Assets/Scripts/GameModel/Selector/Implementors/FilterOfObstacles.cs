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

                if (Shortcut.HasObstacles(this.Selector.transform.Position, target.transform.Position))
                    continue;

                this.filtered.Add(target);
            }

            return this;
        }
    }
}