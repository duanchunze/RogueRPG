namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.AdditionalStatus2)]
    public class PlhHarmAdditionalStatus2 : AProcedureLineHandler<PliDamageArbitramentForm, PlwAdditionalStatusOnAbilityHarm2> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item,
            PlwAdditionalStatusOnAbilityHarm2 worker) {
            // 如果是worker的持有者造成的伤害, 则施加对应的状态
            if (item.source == worker.ProcedureLineNode.Bodied) {
                Shortcut.InflictionStatus(item.harm.Bodied, item.hurt.Bodied, worker.info.StatusAlias, actionInfo: worker.info.Action);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}