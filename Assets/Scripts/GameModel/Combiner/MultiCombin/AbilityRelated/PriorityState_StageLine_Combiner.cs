using System;

namespace Hsenl.MultiCombiner {
    // 该组合会被CasterPriorityStateCombiner覆盖
    public class PriorityState_StageLine_Combiner : MultiCombiner<PriorityState, StageLine> {
        protected override void OnCombin(PriorityState arg1, StageLine arg2) {
            var userToken = ObjectPool.Rent<UserToken>();
            userToken.duration = arg1.duration;
            this.SetUserToken(userToken);
            
            arg1.duration = -1; // 由阶段线来决定持续时间
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
            
            var userToken = this.GetUserToken<UserToken>();
            arg1.duration = userToken.duration;
            ObjectPool.Return(userToken);
        }
        
        private class UserToken {
            public float duration;
        }
    }
}