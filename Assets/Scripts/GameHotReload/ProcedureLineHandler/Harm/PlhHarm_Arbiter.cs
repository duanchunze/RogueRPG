namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.Arbiter)]
    public class PlhHarm_Arbiter : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item) {
            GameArbiter.DamageArbiter(ref item);
            return ProcedureLineHandleResult.Success;
        }
    }
}