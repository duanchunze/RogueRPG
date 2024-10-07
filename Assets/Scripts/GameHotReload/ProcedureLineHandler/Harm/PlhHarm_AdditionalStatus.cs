
namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.AdditionalStatus)]
    public class PlhHarm_AdditionalStatus : AProcedureLineHandler<PliHarmForm, PlwAdditionalStatusOnAbilityHarm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item,
            PlwAdditionalStatusOnAbilityHarm worker, object userToken) {
            Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, worker.info.StatusAlias);

            return ProcedureLineHandleResult.Success;
        }
    }
}