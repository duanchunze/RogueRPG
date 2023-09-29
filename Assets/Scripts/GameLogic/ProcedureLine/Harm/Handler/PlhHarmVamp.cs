namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.Vamp)]
    public class PlhHarmVamp : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            var vamphp = item.finalDamage * item.pvamp;
            if (item.HarmNumerator != null) {
                Shortcut.RecoverHealth(item.HarmNumerator, (int)vamphp);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}