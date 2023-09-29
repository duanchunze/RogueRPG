using System.Collections.Generic;
using Hsenl.timeline;

namespace Hsenl {
    public abstract class TpHarm<T> : TpInfo<T> where T : TpHarmInfo {
        protected List<Numerator> numerators = new(2);
        protected Harmable harmable;
        protected ProcedureLine procedureLine;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = ability.GetHolder()?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    this.harmable = ability.GetHolder()?.GetComponent<Harmable>();
                    this.procedureLine = ability.GetHolder()?.GetComponent<ProcedureLine>();
                    break;
                }
            }
        }

        protected void Harm(Hurtable hurtable, float damageRate = 1f) {
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
                    damage = dmg * damageRate,
                    astun = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Astun),
                };

                this.OnHarm(ref damageForm);
            }
        }

        protected virtual void OnHarm(ref PliDamageArbitramentForm damageArbitramentForm) {
            this.procedureLine.StartLineAsync(damageArbitramentForm).Coroutine();
        }
    }
}