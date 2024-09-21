namespace Hsenl {
    [ProcedureLineHandlerPriority(PliRecoverPriority.AddHpNum)]
    public class PlhRecover_AddHpNum : AProcedureLineHandler<PliRecoverForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliRecoverForm item) {
            Shortcut.RecoverHealth(item.TargetNumerator, item.recoverHp);
            return ProcedureLineHandleResult.Success;
        }
    }
}