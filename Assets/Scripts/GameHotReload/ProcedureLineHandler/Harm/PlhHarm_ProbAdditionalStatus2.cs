namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.AdditionalStatus2)]
    public class PlhHarm_ProbAdditionalStatus2 : AProcedureLineHandler<PliHarmForm, PlwProbAdditionalStatusOnHarm2> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item,
            PlwProbAdditionalStatusOnHarm2 worker, object userToken) {
            var roll = RandomHelper.NextFloat();
            if (roll < worker.info.Probabilistic) {
                Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, worker.info.StatusAlias, worker.info.Duration, worker.info.Action);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}