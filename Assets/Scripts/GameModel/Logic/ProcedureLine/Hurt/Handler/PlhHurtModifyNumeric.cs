namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHurtPriority.ModifyNumeric)]
    public class PlhHurtModifyNumeric : AProcedureLineHandler<PliHurtForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHurtForm item) {
            var hurtNumerator = item.hurtable.Bodied.GetComponent<Numerator>();
            var nowHp = Shortcut.SubtractHealth(hurtNumerator, item.deductHp);
            if (nowHp <= 0) {
                Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, StatusAlias.Death);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}