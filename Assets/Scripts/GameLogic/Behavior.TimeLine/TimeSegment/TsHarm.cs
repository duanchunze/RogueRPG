using System.Collections.Generic;

namespace Hsenl {
    public abstract class TsHarm<T> : TsInfo<T> where T : timeline.TsHarmInfo {
        protected List<Numerator> numerators = new(2);
        protected Harmable harmable;
        protected ProcedureLine procedureLine;

        protected List<(float damage, DamageType damageType, float astun)> damages; // damage, damageType, astun

        // 技能和状态伤害的方式不同, 因为技能我们是可以保证holder一直存在的, 但状态就不行, 可能上完状态后, 这个人就死了, 所以状态是先把伤害存下来, 再使用
        private int holderType; // 0: ability, 1: status

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this.holderType = 0;
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = ability.GetHolder()?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    this.harmable = ability.GetHolder()?.GetComponent<Harmable>();
                    this.procedureLine = ability.GetHolder()?.GetComponent<ProcedureLine>();
                    break;
                }

                case Status: {
                    this.holderType = 1;
                    break;
                }
            }
        }

        protected override void OnNodeReset() {
            switch (this.manager.Substantive) {
                case Status status: {
                    // 状态伤害计算规则
                    // 施加者伤害 + 状态本身伤害 = 伤害数值
                    // 施加者症状强化 - 被施加者症状抵抗 = 伤害强化比例
                    // 状态本身的硬直 = 伤害硬直 (状态造成的伤害硬直, 不会受施加者影响)
                    this.damages ??= new();
                    this.damages.Clear();
                    var inflictorNumerator = status.inflictor.GetComponent<Numerator>();
                    var beinflictorNumerator = status.GetHolder().GetComponent<Numerator>();
                    var statusNumerator = status.GetComponent<Numerator>();
                    var sit = inflictorNumerator.GetValue(NumericType.Sit);
                    var srt = beinflictorNumerator.GetValue(NumericType.Srt);
                    var intensify = sit - srt;
                    var astun = statusNumerator.GetValue(NumericType.Astun);
                    for (int i = 0, len1 = this.info.HarmFormulas.Count; i < len1; i++) {
                        var damageFromulaInfo = this.info.HarmFormulas[i];
                        var dmg = Num.Empty();
                        for (int j = 0, len2 = damageFromulaInfo.DamageFormulas.Count; j < len2; j++) {
                            var formulaInfo = damageFromulaInfo.DamageFormulas[j];
                            var d = GameAlgorithm.MergeCalculateNumeric(inflictorNumerator, statusNumerator, formulaInfo.Type);
                            d *= formulaInfo.Pct;
                            d += formulaInfo.Fix;
                            dmg += d;
                        }

                        dmg -= dmg * intensify;
                        if (dmg <= 0) {
                            dmg = 1;
                        }

                        this.damages.Add((dmg, damageFromulaInfo.DamageType, astun));
                    }

                    this.harmable = status.inflictor.GetComponent<Harmable>();
                    this.procedureLine = status.GetHolder().GetComponent<ProcedureLine>();
                    break;
                }
            }
        }

        protected void Harm(Hurtable hurtable) {
            if (this.holderType == 0) {
                for (int i = 0, len1 = this.info.HarmFormulas.Count; i < len1; i++) {
                    var damageFromulaInfo = this.info.HarmFormulas[i];
                    var dmg = Num.Empty();
                    for (int j = 0, len2 = damageFromulaInfo.DamageFormulas.Count; j < len2; j++) {
                        var formulaInfo = damageFromulaInfo.DamageFormulas[j];
                        var d = GameAlgorithm.MergeCalculateNumeric(this.numerators, formulaInfo.Type);
                        d *= formulaInfo.Pct;
                        d += formulaInfo.Fix;
                        dmg += d;
                    }

                    var damageForm = new PliDamageArbitramentForm {
                        harm = this.harmable,
                        hurt = hurtable,
                        source = this.manager.Substantive,
                        damageType = damageFromulaInfo.DamageType,
                        damage = dmg,
                        astun = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Astun),
                    };

                    this.OnHarm(ref damageForm);
                }
            }
            else if (this.holderType == 1) {
                foreach (var valueTuple in this.damages) {
                    var damageForm = new PliDamageArbitramentForm {
                        harm = this.harmable,
                        hurt = hurtable,
                        source = this.manager.Substantive,
                        damageType = valueTuple.damageType,
                        damage = valueTuple.damage,
                        astun = valueTuple.astun,
                    };

                    this.OnHarm(ref damageForm);
                }
            }
        }

        protected virtual void OnHarm(ref PliDamageArbitramentForm damageArbitramentForm) {
            this.procedureLine.StartLineAsync(damageArbitramentForm).Coroutine();
        }
    }
}