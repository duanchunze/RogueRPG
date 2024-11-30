using System;

namespace Hsenl.MultiCombiner {
    public class ControlTrigger_Caster_Combiner : MultiCombiner<ControlTrigger, Caster> {
        protected override void OnCombin(ControlTrigger arg1, Caster arg2) {
            arg1.onStart += this.EnqueueAction<Action>(() => {
                arg1.GetValue(out var v);
                arg2.castParameter.vector3Value = v;
                arg2.StartCastWithKeepTrying();
            });

            arg1.onSustain += this.EnqueueAction<Action>(() => {
                if (arg1.supportBurstFire) {
                    arg1.GetValue(out var v);
                    arg2.castParameter.vector3Value = v;
                    arg2.StartCast();
                }
            });

            arg1.onEnd += this.EnqueueAction<Action>(arg2.StopCast);
        }

        protected override void OnDecombin(ControlTrigger arg1, Caster arg2) {
            arg1.onStart -= this.DequeueAction<Action>();
            arg1.onSustain -= this.DequeueAction<Action>();
            arg1.onEnd -= this.DequeueAction<Action>();
        }
    }
}