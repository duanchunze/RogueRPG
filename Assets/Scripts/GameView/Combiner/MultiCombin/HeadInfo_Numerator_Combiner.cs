using System;

namespace Hsenl.View.MultiCombiner {
    public class HeadInfo_Numerator_Combiner : MultiCombiner<HeadInfo, Numerator> {
        protected override void OnCombin(HeadInfo arg1, Numerator arg2) {
            arg1.refreshInvoke += this.EnqueueAction<Action>(() => {
                Shortcut.RecoverHealth(arg2, 0);
                Shortcut.RecoverMana(arg2, 0);
            });
        }

        protected override void OnDecombin(HeadInfo arg1, Numerator arg2) {
            arg1.refreshInvoke -= this.DequeueAction<Action>();
        }
    }
}