namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.Vamp)]
    public class PlhHarm_Vamp : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            var vamphp = item.finalDamage * item.pvamp;
            if (item.harmNumerator != null) {
                Shortcut.RecoverHealth(item.harmNumerator, (int)vamphp);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}