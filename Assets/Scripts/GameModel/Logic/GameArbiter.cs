using System;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    // 游戏仲裁
    public static class GameArbiter {
        // 计算防御减伤的除数
        public const float DefenseDiv = 200f;
        public const float CritDiv = 200f;

        // 伤害仲裁
        public static void DamageArbiter(ref PliDamageArbitramentForm form) {
            var finalDamage = form.damage;

            switch (form.damageType) {
                case DamageType.PhysicalDamage: {
                    // -----------------计算是否会命中
                    var mzl = form.dex - form.eva;
                    var mzl_roll = RandomHelper.mtRandom.NextFloat();
                    if (mzl_roll > mzl) {
                        form.iseva = true;
                        break;
                    }

                    // 计算基础伤害值, 这是物理伤害的算法, 如果是魔法伤害的话, 就直接用damage - 对方的魔抗. 当然, 目前游戏移除了魔法伤害相关内容
                    if (form.amr > 0) {
                        var subDamagePct = form.amr / (form.amr + DefenseDiv); // 减伤比例 = 防御 / （防御 + const）
                        finalDamage *= (1 - subDamagePct);
                    }
                    else if (form.amr < 0) {
                        // 当防御降到0以下，每降100点，基础伤害增加1倍
                        finalDamage *= (math.abs(form.amr) * 0.01f + 1);
                    }

                    // 暴击
                    var bjl = form.pcrit;
                    var bjl_roll = RandomHelper.NextFloat();
                    if (bjl_roll <= bjl) {
                        finalDamage *= form.pcit;
                        form.ispcrit = true;
                    }

                    // 背击
                    var harmDir = form.harm.transform.Position - form.hurt.transform.Position;
                    var harmAngle = Vector3.Angle(harmDir, form.hurt.transform.Forward);
                    if (harmAngle > 100) {
                        form.backhit = true;
                        finalDamage *= 1.2f;
                    }

                    // 伤害波动
                    var fluctuate = RandomHelper.mtRandom.NextFloat(0.95f, 1.05f);
                    finalDamage *= fluctuate;
                    form.fluctuate = fluctuate;

                    // 格挡
                    var blk_roll = RandomHelper.NextFloat();
                    if (blk_roll < form.blk) {
                        finalDamage *= 0.4f;
                        form.isblk = true;
                    }

                    // 计算评分
                    form.score = (fluctuate - 0.95f) / 0.1f;

                    form.finalDamage = (int)finalDamage;
                    break;
                }
                case DamageType.LightDamage: {
                    form.finalDamage = (int)(finalDamage - form.lrt);
                    break;
                }
                case DamageType.DarkDamage: {
                    form.finalDamage = (int)(finalDamage - form.drt);
                    break;
                }
                case DamageType.FireDamage: {
                    form.finalDamage = (int)(finalDamage - form.frt);
                    break;
                }
                case DamageType.IceDamage: {
                    form.finalDamage = (int)(finalDamage - form.irt);
                    break;
                }
                case DamageType.TrueDamage: {
                    form.finalDamage = (int)finalDamage;
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            form.finalDamage = math.clamp(form.finalDamage, 1, int.MaxValue);
        }
    }
}