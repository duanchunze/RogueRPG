using System;

namespace Hsenl.MultiCombiner {
    public class Prioritizer_Minion_Combiner : MultiCombiner<Prioritizer, Minion> {
        protected override void OnCombin(Prioritizer arg1, Minion arg2) {
            arg1.OnStateEnter += this.EnqueueAction<Action<IPriorityState>>((priorityState) => {
                if (priorityState.Name == StatusAlias.Death) {
                    arg2.OnOver();
                }
            });
        }

        protected override void OnDecombin(Prioritizer arg1, Minion arg2) {
            arg1.OnStateEnter -= this.DequeueAction<Action<IPriorityState>>();
        }
    }
}