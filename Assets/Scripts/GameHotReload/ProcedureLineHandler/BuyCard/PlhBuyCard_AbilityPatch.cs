namespace Hsenl {
    [ProcedureLineHandlerPriority(PliBuyCardPriority.AbilityPatch)]
    public class PlhBuyCard_AbilityPatch : AProcedureLineHandler<PliBuyCardForm> {
        // 如果是技能补丁的话, 就直接放到技能下面, 途省事这么写
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item, object userToken) {
            if (item.target is AbilityPatch abilityPatch) {
                var abiBar = procedureLine.Bodied.FindBodiedInIndividual<AbilitesBar>();
                var config = abilityPatch.Config;
                var abi = abiBar.FindAbility(config.TargetAbility);
                if (abi != null) {
                    abi.AddPatch(abilityPatch);
                    return ProcedureLineHandleResult.Break;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}