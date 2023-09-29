using System;

namespace Hsenl.MultiCombiner {
    // 过时了, 该组合会被CasterPriorityStateCombiner覆盖
    public class PriorityStateStageLineCombiner : MultiCombiner<PriorityState, StageLine> {
        protected override void OnCombin(PriorityState arg1, StageLine arg2) {
            arg1.duration = -1; // 优先器的持续时间, 交给阶段线来决定
            arg1.onEnter += this.EnqueueAction<Action<IPrioritizer>>(_ => { arg2.Reset(); });
            arg1.onUpdate += this.EnqueueAction<Action<IPrioritizer, float>>((_, deltaTime) => {
                var status = arg2.Run(deltaTime);
                if (status != StageStatus.Running) {
                    arg1.LeaveState();
                }
            });
            arg1.onLeave += this.EnqueueAction<Action<IPrioritizer>>(_ => { arg2.Abort(); });
        }

        protected override void OnDecombin(PriorityState arg1, StageLine arg2) {
            arg1.onEnter -= this.DequeueAction<Action<IPrioritizer>>();
            arg1.onUpdate -= this.DequeueAction<Action<IPrioritizer, float>>();
            arg1.onLeave -= this.DequeueAction<Action<IPrioritizer>>();
        }
    }
}