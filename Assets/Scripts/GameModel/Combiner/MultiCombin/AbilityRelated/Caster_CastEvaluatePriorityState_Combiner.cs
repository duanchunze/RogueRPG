using System;

namespace Hsenl.MultiCombiner {
    public class Caster_CastEvaluatePriorityState_Combiner : MultiCombiner<Caster, CastEvaluatePriorityState> {
        protected override void OnCombin(Caster arg1, CastEvaluatePriorityState arg2) {
            arg1.startKeepTryInvoke += this.EnqueueAction<Action>(() => { arg2.Enter(); });
            arg1.isKeepTringInvoke += this.EnqueueAction<Func<bool>>(() => arg2.IsEntered);
            arg1.stopKeepTringInvoke += this.EnqueueAction<Action>(() => { arg2.Leave(); });
            arg2.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((_, deltaTime) => {
                if (!Timer.ClockTick(0.03f))
                    return;

                var state = arg1.Evaluate();
                if (state != CastEvaluateState.Success) {
                    if (state != CastEvaluateState.Trying) {
                        // try 失败
                        arg2.Leave();
                        return;
                    }

                    // 继续保持 try
                    return;
                }

                // try 成功了
                arg1.DirectStartCast();
            });
        }

        protected override void OnDecombin(Caster arg1, CastEvaluatePriorityState arg2) {
            arg1.startKeepTryInvoke -= this.DequeueAction<Action>();
            arg1.isKeepTringInvoke -= this.DequeueAction<Func<bool>>();
            arg1.stopKeepTringInvoke -= this.DequeueAction<Action>();
            arg2.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
        }
    }
}