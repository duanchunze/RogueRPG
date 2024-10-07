using System;
using Hsenl.EventType;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityStageChangedPriority.Wuqishou)]
    public class PlhAbilityStageChanged_Wuqishou : AProcedureLineHandler<PliAbilityStageChangedForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityStageChangedForm item, object userToken) {
            var abi = item.ability;
            switch ((StageType)item.currStage) {
                case StageType.None:
                    break;
                case StageType.Enter: {
                    break;
                }
                case StageType.Reading: {
                    break;
                }
                case StageType.Charging: {
                    break;
                }
                case StageType.Lifting: {
                    // 如果技能的施法者有"无起手"状态, 那就把起手时间归零, 且给他一个BufferSpeed的提速
                    var status = abi.MainBodied.FindBodiedInIndividual<StatusBar>().GetStatus(StatusAlias.Wuqishou);
                    if (status is { IsEnter: true }) {
                        item.stageLine.BufferSpeed = 5.2f;
                        item.stageLine.StageTillTime = 0;
                        status.Finish();
                    }

                    break;
                }
                case StageType.Casting: {
                    break;
                }
                case StageType.Recovering:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}