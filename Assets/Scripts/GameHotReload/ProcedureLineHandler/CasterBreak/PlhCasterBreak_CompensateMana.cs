using Hsenl.EventType;
using UnityEngine;

namespace Hsenl.Handler {
    [ProcedureLineHandlerPriority(PliCasterBreakPriority.CompensateMana)]
    public class PlhCasterBreak_CompensateMana : AProcedureLineHandler<PliCasterBreakForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliCasterBreakForm item, object userToken) {
            switch (item.caster.Bodied) {
                case Ability ability: {
                    
                    // todo 以下的补偿都可以做成状态, 然后使用worker来处理, 这里处理比较临时, 不规范
                    // 最新一次修改, 把耗蓝以及冷却都放到casting里执行了, 代表只有打出伤害了, 才算是释放成功, 所以这里也就没必要补偿什么了, 因为压根就没计算cd
                    // var stageLine = ability.GetComponent<StageLine>();
                    // if (stageLine.CurrentStage is (int)StageType.Reading or (int)StageType.Charging or (int)StageType.Lifting) {
                    //     // 如果是上面的过程中被打断, 则对下次释放时, 对冷却进行补偿
                    //     ability.cooldownCompensate += 0.15f;
                    // }

                    // if (ability.Tags.Contains(TagType.AbilityAttack)) {
                    //     // 如果起手的时候被打断了, 则对下次的释放速度进行一次补偿
                    //     if (stageLine.CurrentStage is (int)StageType.Lifting) {
                    //         ability.casterCompensate += 0.15f;
                    //     }
                    // }

                    break;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}