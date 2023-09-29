using UnityEngine;

namespace Hsenl {
    // 造成伤害时, 概率触发施法器
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.CastTrigger)]
    public class PlhHarmProbabilisticCast : AProcedureLineHandler<PliDamageArbitramentForm, CwHarmProbabilisticCast> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item, CwHarmProbabilisticCast worker) {
            var random = RandomHelper.mtRandom.NextFloat();
            if (random > worker.info.Probability) {
                worker.caster.CastStart();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}