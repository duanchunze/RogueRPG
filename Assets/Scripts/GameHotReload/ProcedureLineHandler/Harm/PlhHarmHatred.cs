namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.Hatred)]
    public class PlhHarmHatred : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            var selections = item.hurt.GetComponent<Selector>();
            if (selections.PrimarySelection == null) {
                selections.PrimarySelection = item.harm.GetComponent<SelectionTarget>();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}