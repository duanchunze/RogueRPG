namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.Hatred)]
    public class PlhHarm_Hatred : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            var selections = item.hurtable.GetComponent<Selector>();
            if (selections.PrimarySelection == null) {
                selections.PrimarySelection = item.harmable.GetComponent<SelectionTarget>();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}