namespace Hsenl {
    // 造成伤害时, 概率触发施法器
    [ProcedureLineHandlerPriority(PliHarmPriority.CastTrigger)]
    public class PlhHarm_ProbabilisticCast : AProcedureLineHandler<PliHarmForm, PlwCastOfHarmProbabilistic> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, PlwCastOfHarmProbabilistic worker, object userToken) {
            var random = RandomHelper.NextFloat();
            if (random <= worker.info.Probability) {
                worker.WorkerHolder.GetComponent<Caster>().CastStart(true);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}