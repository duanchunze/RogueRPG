using System;

namespace Hsenl.MultiCombiner {
    public class CasterCasterEvaluateCombiner : MultiCombiner<Caster, CasterEvaluate> {
        protected override void OnCombin(Caster arg1, CasterEvaluate arg2) {
            arg1.evaluateInvokes += this.EnqueueAction<Func<float, CastEvaluateStatus>>(arg2.Evaluate);
        }

        protected override void OnDecombin(Caster arg1, CasterEvaluate arg2) {
            arg1.evaluateInvokes -= this.DequeueAction<Func<float, CastEvaluateStatus>>();
        }
    }
}