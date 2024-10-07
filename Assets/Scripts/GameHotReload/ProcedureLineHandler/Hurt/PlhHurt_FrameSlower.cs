namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHurtPriority.FrameSlower)]
    public class PlhHurt_FrameSlower : AProcedureLineHandler<PliHurtForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHurtForm item, object userToken) {
            if (item.frameSlower) {
                var abilityHolder = item.hurtable.Bodied.FindBodiedInIndividual<AbilitesBar>();
                var bufferSpeed = GameFormula.CalculateFrameSlower(item.astun);
                foreach (var ability in abilityHolder.currentCastingAbilities) {
                    // todo 这里应该加个判断, 不是所有的技能都会被减速, 只有带有允许被帧减速标签的技能才可以.
                    var stageLine = ability.GetComponent<StageLine>();
                    stageLine.BufferSpeed = bufferSpeed;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}