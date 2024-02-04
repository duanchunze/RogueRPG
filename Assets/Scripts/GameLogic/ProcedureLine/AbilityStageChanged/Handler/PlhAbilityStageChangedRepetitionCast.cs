namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityStageChangedPriority.RepetitionCast)]
    public class PlhAbilityStageChangedRepetitionCast : AProcedureLineHandler<PliAbilityStageChangedForm, PlwRepetitionCastOfProbabilistic> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityStageChangedForm item, PlwRepetitionCastOfProbabilistic worker) {
            if (item.ability != worker.ProcedureLineNode.Bodied) {
                return ProcedureLineHandleResult.Success;
            }
            
            switch ((StageType)item.currStage) {
                case StageType.Enter: {
                    var random = RandomHelper.mtRandom.NextFloat();
                    if (random <= worker.info.Probability) {
                        // 重复释放一次
                        // 代理释放的话有两种方案.
                        // 第一种是创建一个新的技能, 该技能为代理技能, 然后重新释放一次, 这种方案走prioriter, 走cast, 是最完整的代理释放方案.
                        // 缺点就是麻烦, 需要拷贝一个新的技能, 且每次释放都需要拷贝一个.
                        // 第二种是就是仅把技能的behavior tree重新执行一遍, 由外部开启一个协程驱动, 好处是方便, 坏处是这个代理技能没有走prioriter, 所以不受控制, 且会和原技能共用变量.
                        // 比如玩家在触发了代理技能施法的时候, 突然被虚空的大定住了, 此时代理施法依然会正常进行, 而不会被打断, 而如果是第一种方案的话, 代理释放很自然的会被打断.
                        // 综合考虑, 应该使用第一种方案, 更合理一些.
                    }

                    break;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}