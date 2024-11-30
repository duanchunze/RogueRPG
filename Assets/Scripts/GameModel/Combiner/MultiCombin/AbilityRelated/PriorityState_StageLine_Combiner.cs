using System;

namespace Hsenl.MultiCombiner {
    public class PriorityState_StageLine_Combiner : MultiCombiner<PriorityState, StageLine> {
        private class Variable {
            public float duration;
        }

        protected override void OnCombin(PriorityState arg1, StageLine arg2) {
            var variable = this.GetUserToken<Variable>();
            variable.duration = arg1.Duration;

            arg1.Duration = -1; // 由阶段线来决定持续时间
            arg1.onEnter += this.EnqueueAction<Action<IPrioritizer>>(_ => { arg2.Start(); });
            arg1.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((_, deltaTime) => {
                var status = arg2.Run(deltaTime);
                if (status != StageStatus.Running) {
                    arg1.Leave();
                }
            });
            arg1.onLeave += this.EnqueueAction<Action<IPrioritizer>>(_ => { arg2.Abort(); });
        }

        protected override void OnDecombin(PriorityState arg1, StageLine arg2) {
            arg1.onEnter -= this.DequeueAction<Action<IPrioritizer>>();
            arg1.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
            arg1.onLeave -= this.DequeueAction<Action<IPrioritizer>>();

            var variable = this.GetUserToken<Variable>();
            arg1.Duration = variable.duration;
        }
    }
}