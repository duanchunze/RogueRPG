using System;

namespace Hsenl.MultiCombiner {
    public class StatusProcedureLineCombiner : CrossCombiner<Status, ProcedureLine> {
        protected override void OnCombin(Status arg1, ProcedureLine arg2) {
            arg1.onBegin += this.EnqueueAction<Action>(() => {
                var form = new PliStatusChangesForm() {
                    changeType = 0,
                    inflictor = arg1.inflictor,
                    target = arg1.GetHolder(),
                    statusAlias = arg1.Name,
                    duration = arg1.GetComponent<TimeLine>().TillTime,
                };

                arg2.StartLine(ref form);
            });
            arg1.onFinish += this.EnqueueAction<Action<StatusFinishDetails>>((details) => {
                var form = new PliStatusChangesForm() {
                    changeType = 1,
                    inflictor = arg1.inflictor,
                    target = arg1.GetHolder(),
                    statusAlias = arg1.Name,
                    finishDetails = details,
                };

                arg2.StartLine(ref form);
            });
        }

        protected override void OnDecombin(Status arg1, ProcedureLine arg2) {
            arg1.onBegin -= this.DequeueAction<Action>();
            arg1.onFinish -= this.DequeueAction<Action<StatusFinishDetails>>();
        }
    }
}