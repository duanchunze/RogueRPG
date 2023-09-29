using System;

namespace Hsenl.CrossCombiner {
    public class ProcedureLineNodeProcedureLineCombiner : CrossCombiner<ProcedureLineNode, ProcedureLine> {
        protected override void OnCombin(ProcedureLineNode arg1, ProcedureLine arg2) {
            arg2.AddWorker(arg1);
            arg1.onWorkerAdd += this.EnqueueAction<Action<IProcedureLineWorker>>(arg2.AddWorker);
            arg1.onWorkerRemove += this.EnqueueAction<Action<IProcedureLineWorker>>(arg2.RemoveWorker);
        }

        protected override void OnDecombin(ProcedureLineNode arg1, ProcedureLine arg2) {
            arg2.RemoveWorker(arg1);
            arg1.onWorkerAdd -= this.DequeueAction<Action<IProcedureLineWorker>>();
            arg1.onWorkerRemove -= this.DequeueAction<Action<IProcedureLineWorker>>();
        }
    }
}