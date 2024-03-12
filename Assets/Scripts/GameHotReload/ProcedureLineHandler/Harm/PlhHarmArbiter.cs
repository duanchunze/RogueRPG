namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.Arbiter)]
    public class PlhHarmArbiter : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            GameArbiter.DamageArbiter(ref item);
            return ProcedureLineHandleResult.Success;
        }
    }
}