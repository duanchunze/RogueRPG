using System;
using Hsenl.EventType;
using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityStageChangedPriority.ModifyArg)]
    public class PlhAbilityStageChanged : AProcedureLineHandler<PliAbilityStageChangedForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityStageChangedForm item) {
            var abi = item.ability;
            switch ((StageType)item.currStage) {
                case StageType.None:
                    break;
                case StageType.Enter: {
                    EventStation.OnAbilityCasted(item.Spellcaster, abi);
                    break;
                }
                case StageType.Reading: {
                    break;
                }
                case StageType.Charging: {
                    break;
                }
                case StageType.Lifting: {
                    var status = abi.MainBodied.FindBodiedInIndividual<StatusBar>().GetStatus(StatusAlias.Wuqishou);
                    if (status is { IsEnter: true }) {
                        item.stageLine.BufferSpeed = 5.2f;
                        item.stageLine.StageTillTime = 0;
                        status.Finish();
                    }
                    else {
                        if (abi.casterCompensate > 0) {
                            item.stageLine.StageTillTime -= item.stageLine.StageTillTime * abi.casterCompensate;
                            if (item.stageLine.StageTillTime < 0) {
                                item.stageLine.StageTillTime = 0;
                            }
                        }

                        // 攻速影响起手速度
                        // 2023.8.27日, 修改了, 游戏不在有攻速属性, 目前的考虑是, 初始已经可以实现同时释放6个普攻技能, 加攻速意义就不大了
                        // var aspd = GameAlgorithm.MergeCalculateNumeric(item.CasterNumerator, item.AbilityNumerator, NumericType.Aspd);
                        // item.stageLine.Speed = aspd;
                    }

                    break;
                }
                case StageType.Casting: {
                    item.manaCost = item.AbilityNumerator.GetValue(NumericType.ManaCost);
                    item.cd = item.AbilityNumerator.GetValue(NumericType.CD);
                    if (item.manaCost != 0) {
                        if (item.manaCost > 0) {
                            Shortcut.SubtractMana(item.SpellcasterNumerator, item.manaCost);
                        }
                        else {
                            Shortcut.RecoverMana(item.SpellcasterNumerator, -item.manaCost);
                        }
                    }

                    // 开始进入冷却. 法术极速影响所有技能的冷却
                    var cooldown = item.cd;
                    var abiHaste = GameAlgorithm.MergeCalculateNumeric(item.AbilityNumerator, item.SpellcasterNumerator, NumericType.Ahaste);
                    cooldown = GameFormula.CalculateCooldownTime(cooldown, abiHaste);
                    if (abi.cooldownCompensate > 0) {
                        cooldown -= cooldown * abi.cooldownCompensate;
                        if (cooldown < 0) cooldown = 0;
                    }

                    var cooltilltime = TimeInfo.Time + cooldown;
                    abi.Cooldown(cooltilltime);
                    EventStation.OnAbilityCooldown(abi, cooldown, cooltilltime);

                    if (abi.casterCompensate > 0) {
                        abi.casterCompensate = 0;
                    }

                    if (abi.cooldownCompensate > 0) {
                        abi.cooldownCompensate = 0;
                    }

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