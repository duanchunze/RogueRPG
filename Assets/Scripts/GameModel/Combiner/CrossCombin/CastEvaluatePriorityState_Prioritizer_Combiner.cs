namespace Hsenl.CrossCombiner {
    public class CastEvaluatePriorityState_Prioritizer_Combiner : CrossCombiner<CastEvaluatePriorityState, Prioritizer> {
        protected override void OnCombin(CastEvaluatePriorityState arg1, Prioritizer arg2) {
            arg1.TargetPrioritizer = arg2;
        }

        protected override void OnDecombin(CastEvaluatePriorityState arg1, Prioritizer arg2) {
            arg1.TargetPrioritizer = null;
        }
    }
}