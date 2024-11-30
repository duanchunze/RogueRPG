
namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.AdditionalStatus)]
    public class PlhHarm_ProbAdditionalStatus : AProcedureLineHandler<PliHarmForm, PlwProbAdditionalStatusOnHarm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item,
            PlwProbAdditionalStatusOnHarm worker, object userToken) {
            var roll = RandomHelper.NextFloat();
            if (roll < worker.info.Probabilistic) {
                Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, worker.info.StatusAlias);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}