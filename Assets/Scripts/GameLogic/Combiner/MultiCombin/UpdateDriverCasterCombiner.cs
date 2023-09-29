using System;

namespace Hsenl.MultiCombiner {
    public class UpdateDriverCasterCombiner : MultiCombiner<UpdateDriver, Caster> {
        protected override void OnCombin(UpdateDriver arg1, Caster arg2) {
            arg1.updateInvoke += this.EnqueueAction<Action>(() => {
                arg2.CastStart();
            });
        }

        protected override void OnDecombin(UpdateDriver arg1, Caster arg2) {
            arg1.updateInvoke -= this.DequeueAction<Action>();
        }
    }
}