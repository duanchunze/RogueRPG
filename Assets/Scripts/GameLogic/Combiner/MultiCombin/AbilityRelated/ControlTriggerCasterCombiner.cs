using System;

namespace Hsenl.MultiCombiner {
    public class ControlTriggerCasterCombiner : MultiCombiner<ControlTrigger, Caster> {
        protected override void OnCombin(ControlTrigger arg1, Caster arg2) {
            arg1.onBegin += this.EnqueueAction<Action>(() => { arg2.CastStart(true); });

            arg1.onFinish += this.EnqueueAction<Action>(() => { 
                // 当施法器的模式是无限时间的时候, 才由控制器控制结束
                if (arg2.CastModel == CastModel.InfiniteTime) {
                    arg2.CastEnd();
                }
            });
        }

        protected override void OnDecombin(ControlTrigger arg1, Caster arg2) {
            arg1.onBegin -= this.DequeueAction<Action>();
            arg1.onFinish -= this.DequeueAction<Action>();
        }
    }
}