using System;
using Hsenl.EventType;
using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityCastChangedPriority.ModifyArg)]
    public class PlhAbilityCast : AProcedureLineHandler<PliAbilityCastChangedForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityCastChangedForm item) {
            var abi = item.ability;
            switch ((StageType)item.currStage) {
                case StageType.None:
                    break;
                case StageType.Enter: {
                    EventSystem.Publish(new OnAbilityCasted() { caster = item.caster, ability = abi });

                    break;
                }
                case StageType.Reading: {
                    break;
                }
                case StageType.Charging: {
                    break;
                }
                case StageType.Lifting: {
                    var status = abi.GetHolder().FindSubstaintiveInChildren<StatusBar>().GetStatus(StatusAlias.Wuqishou);
                    if (status is { IsEnter: true }) {
                        item.stageLine.BufferSpeed = 5.2f;
                        item.stageLine.TillTime = 0;
                        status.Finish();
                    }
                    else {
                        if (abi.casterCompensate > 0) {
                            item.stageLine.TillTime -= item.stageLine.TillTime * abi.casterCompensate;
                            if (item.stageLine.TillTime < 0) {
                                item.stageLine.TillTime = 0;
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
                    if (abi.manaCost != 0) {
                        if (abi.manaCost > 0) {
                            Shortcut.SubtractMana(item.CasterNumerator, abi.manaCost);
                        }
                        else {
                            Shortcut.RecoverMana(item.CasterNumerator, -abi.manaCost);
                        }
                    }
                    
                    // 开始进入冷却. 法术极速影响所有技能的冷却
                    var cooldown = abi.Config.Cooldown;
                    var abiHaste = GameAlgorithm.MergeCalculateNumeric(item.AbilityNumerator, item.CasterNumerator, NumericType.Ahaste);
                    cooldown = GameFormula.CalculateCooldownTime(cooldown, abiHaste);
                    if (abi.cooldownCompensate > 0) {
                        cooldown -= cooldown * abi.cooldownCompensate;
                        if (cooldown < 0) cooldown = 0;
                    }

                    var cooltilltime = TimeInfo.Time + cooldown;
                    abi.ResetCooldown(cooltilltime);
                    EventSystem.Publish(new OnAbilityCooldown() { ability = abi, cooltime = cooldown, cooltilltime = cooltilltime });

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