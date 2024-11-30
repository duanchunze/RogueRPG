namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.Hatred)]
    public class PlhHarm_Hatred : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            var selector = item.hurtable.GetComponent<SelectorDefault>();
            if (selector.PrimaryTarget == null) {
                selector.PrimaryTarget = item.harmable.GetComponent<SelectionTargetDefault>();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}