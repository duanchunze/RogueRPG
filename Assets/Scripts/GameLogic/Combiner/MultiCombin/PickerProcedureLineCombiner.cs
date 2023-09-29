using System;

namespace Hsenl.MultiCombiner {
    public class PickerProcedureLineCombiner : MultiCombiner<Picker, ProcedureLine> {
        protected override void OnCombin(Picker arg1, ProcedureLine arg2) {
            arg1.onPickUp += this.EnqueueAction<Action<Pickable>>(pickable => {
                var form = new PliPickupForm {
                    picker = arg1,
                    pickable = pickable,
                };

                arg2.StartLine(ref form);
            });
        }

        protected override void OnDecombin(Picker arg1, ProcedureLine arg2) {
            arg1.onPickUp -= this.DequeueAction<Action<Pickable>>();
        }
    }
}