using Hsenl;

namespace Hsenl.CrossCombiner {
    public class PriorityState_Prioritizer_Combiner : CrossCombiner<PriorityState, Prioritizer> {
        protected override void OnCombin(PriorityState arg1, Prioritizer arg2) {
            arg1.TargetPrioritizer = arg2;
            
            if (arg1.Tags.Contains(TagType.AbilityIdle)) {
                arg2.SetDefaultPriorityState(arg1);
            }
        }

        protected override void OnDecombin(PriorityState arg1, Prioritizer arg2) {
            arg1.TargetPrioritizer = null;
            
            if (arg1.Tags.Contains(TagType.AbilityIdle)) {
                arg2.RemoveDefaultPriorityState(arg1);
            }
        }
    }
}