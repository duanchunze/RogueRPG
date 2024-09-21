using System;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.CalcNumeric)]
    public class PlhHarm_CalcNumeric : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item) {
            // 伤害类型和伤害值
            if (item.damageFormulaInfo != null) {
                switch (item.source) {
                    case Status:
                    case Ability: {
                        item.damageType = item.damageFormulaInfo.DamageType;
                        for (int i = 0, len = item.damageFormulaInfo.DamageFormulas.Count; i < len; i++) {
                            var formulaInfo = item.damageFormulaInfo.DamageFormulas[i];
                            var d = GameAlgorithm.MergeCalculateNumeric(item.harmNumerator, item.sourceNumerator, formulaInfo.Type);
                            d *= formulaInfo.Pct;
                            d += formulaInfo.Fix;
                            item.damage += d;
                        }

                        break;
                    }
                }
            }

            // 状态伤害还和强化和抵抗有关
            {
                if (item.source is Status) {
                    // 状态伤害计算规则
                    // 施加者伤害 + 状态本身伤害 = 伤害数值
                    // 施加者症状强化 - 被施加者症状抵抗 = 伤害强化比例
                    var sit = item.harmNumerator?.GetValue(NumericType.Sit) ?? Num.Empty();
                    var srt = item.hurtNumerator.GetValue(NumericType.Srt);
                    var intensify = sit - srt;
                    item.damage -= item.damage * intensify;
                }
            }
            
            // 乘以伤害比例
            {
                if (item.damageRatio == 0)
                    Log.Warning("damage ratio == 0");
                item.damage *= item.damageRatio;
            }

            // 攻击硬直和受击硬直
            {
                switch (item.source) {
                    case Status: {
                        item.astun = item.sourceNumerator.GetValue(NumericType.Astun); // 状态造成的硬直只与状态本身有关
                        break;
                    }
                    case Ability: {
                        // 技能造成的硬直, 与人物和技能本身有关
                        item.astun = GameAlgorithm.MergeCalculateNumeric(item.harmNumerator, item.sourceNumerator, NumericType.Astun);
                        break;
                    }
                }

                item.hstun += item.hurtNumerator.GetValue(NumericType.Hstun);
            }

            // 计算影响伤害相关的数值
            switch (item.damageType) {
                case DamageType.PhysicalDamage:
                    if (item.harmNumerator != null) {
                        item.dex += item.harmNumerator.GetValue(NumericType.Dex);
                        item.pcrit += item.harmNumerator.GetValue(NumericType.Pcirt);
                        item.pcit += item.harmNumerator.GetValue(NumericType.Pcit);
                        item.pvamp += item.harmNumerator.GetValue(NumericType.Pvamp);
                    }

                    item.amr += item.hurtNumerator.GetValue(NumericType.Amr);
                    item.eva += item.hurtNumerator.GetValue(NumericType.Eva);
                    item.blk += item.hurtNumerator.GetValue(NumericType.Blk);
                    break;
                case DamageType.TrueDamage:
                    break;
                case DamageType.LightDamage:
                    item.lrt = item.hurtNumerator.GetValue(NumericType.Lrt);
                    break;
                case DamageType.DarkDamage:
                    item.drt = item.hurtNumerator.GetValue(NumericType.Drt);
                    break;
                case DamageType.FireDamage:
                    item.frt = item.hurtNumerator.GetValue(NumericType.Frt);
                    break;
                case DamageType.IceDamage:
                    item.irt = item.hurtNumerator.GetValue(NumericType.Irt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}