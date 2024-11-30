using System;
using Hsenl.EventType;
using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliAbilityStageChangedPriority.StandardHandle)]
    public class PlhAbilityStageChanged : AProcedureLineHandler<PliAbilityStageChangedForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityStageChangedForm item, object userToken) {
            var abi = item.ability;
            switch ((StageType)item.currStage) {
                case StageType.None:
                    break;
                case StageType.Enter: {
                    EventStation.OnAbilityCasted(item.Spellcaster, abi);
                    break;
                }
                case StageType.Reading: {
                    var cspd = GameAlgorithm.MergeCalculateNumeric(item.SpellcasterNumerator, item.AbilityNumerator, NumericType.Cspd);
                    item.stageLine.Speed = cspd;
                    break;
                }
                case StageType.Charging: {
                    item.stageLine.Speed = 1f;
                    break;
                }
                case StageType.Lifting: {
                    var status = abi.MainBodied.FindBodiedInIndividual<StatusBar>().GetStatus(StatusAlias.Wuqishou);
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

                        var aspd = GameAlgorithm.MergeCalculateNumeric(item.SpellcasterNumerator, item.AbilityNumerator, NumericType.Aspd);
                        item.stageLine.Speed = aspd;
                    }

                    break;
                }
                case StageType.Casting: {
                    // 蓝耗
                    {
                        item.manaCost = item.AbilityNumerator.GetValue(NumericType.ManaCost);
                        if (item.manaCost != 0) {
                            if (item.manaCost > 0) {
                                Shortcut.SubtractMana(item.SpellcasterNumerator, item.manaCost);
                            }
                            else {
                                Shortcut.RecoverMana(item.SpellcasterNumerator, -item.manaCost);
                            }
                        }
                    }

                    // 开始进入冷却. 法术极速影响所有技能的冷却
                    {
                        item.cd = item.AbilityNumerator.GetValue(NumericType.CD);
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