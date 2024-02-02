using Hsenl.Cast;

namespace Hsenl {
    // 造成伤害时, 概率触发施法器
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.CastTrigger)]
    public class PlhHarmProbabilisticCast : AProcedureLineHandler<PliDamageArbitramentForm, PlwCastOfHarmProbabilistic> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item, PlwCastOfHarmProbabilistic worker) {
            var random = RandomHelper.mtRandom.NextFloat();
            if (random > worker.info.Probability) {
                worker.ProcedureLineNode.GetComponent<Caster>().CastStart(true);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}