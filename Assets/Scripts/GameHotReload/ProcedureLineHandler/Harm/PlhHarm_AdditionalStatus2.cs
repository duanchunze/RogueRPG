namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.AdditionalStatus2)]
    public class PlhHarm_AdditionalStatus2 : AProcedureLineHandler<PliHarmForm, PlwAdditionalStatusOnAbilityHarm2> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item,
            PlwAdditionalStatusOnAbilityHarm2 worker) {
            Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, worker.info.StatusAlias, worker.info.Duration, worker.info.Action);

            return ProcedureLineHandleResult.Success;
        }
    }
}