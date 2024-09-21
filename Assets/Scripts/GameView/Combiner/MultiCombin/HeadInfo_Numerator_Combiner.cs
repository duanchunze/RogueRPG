using System;

namespace Hsenl.View.MultiCombiner {
    public class HeadInfo_Numerator_Combiner : MultiCombiner<HeadInfo, Numerator> {
        protected override void OnCombin(HeadInfo arg1, Numerator arg2) {
            arg1.refreshInvoke += this.EnqueueAction<Action>(() => {
                var hpPct = Shortcut.GetHealthPct(arg2);
                arg1.UpdateHp(hpPct);

                var manaPct = Shortcut.GetManaPct(arg2);
                arg1.UpdateMana(manaPct);
            });
        }

        protected override void OnDecombin(HeadInfo arg1, Numerator arg2) {
            arg1.refreshInvoke -= this.DequeueAction<Action>();
        }
    }
}