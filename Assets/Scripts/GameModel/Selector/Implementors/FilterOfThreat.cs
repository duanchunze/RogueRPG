using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public class FilterOfThreat : ASelectionsFilter {
        public override ASelectionsFilter Filter(IReadOnlyList<SelectionTarget> sts) {
            this.filtered.Clear();

            for (int i = 0, len = sts.Count; i < len; i++) {
                var target = sts[i];

                var numerator = this.Selector.GetComponent<Numerator>();
                var targetNumerator = target.GetComponent<Numerator>();
                if (numerator == null || targetNumerator == null)
                    continue;
                
                var threat = GameAlgorithm.ComparisonThreat(numerator, targetNumerator);
                if (!threat)
                    continue;

                this.filtered.Add(target);
            }

            return this;
        }
    }
}