
namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.AdditionalStatus)]
    public class PlhHarmAdditionalStatus : AProcedureLineHandler<PliDamageArbitramentForm, PlwAdditionalStatusOnAbilityHarm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item,
            PlwAdditionalStatusOnAbilityHarm worker) {
            // 如果是worker的持有者造成的伤害, 则施加对应的状态
            if (item.source == worker.ProcedureLineNode.Bodied) {
                Shortcut.InflictionStatus(item.harm.Bodied, item.hurt.Bodied, worker.info.StatusAlias);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}