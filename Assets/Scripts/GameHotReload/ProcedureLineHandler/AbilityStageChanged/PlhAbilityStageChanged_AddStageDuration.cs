using System;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityStageChangedPriority.AddStageDuration)]
    public class PlhAbilityStageChanged_AddStageDuration : AProcedureLineHandler<PliAbilityStageChangedForm, PlwAddStageDuration> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityStageChangedForm item, PlwAddStageDuration worker,
            object userToken) {
            var currStage = (StageType)item.currStage;
            if (currStage == worker.info.StageType) {
                var tillTime = item.stageLine.TillTime;
                tillTime *= worker.info.Pct;
                tillTime += worker.info.Fix;
                item.stageLine.TillTime = tillTime;
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}