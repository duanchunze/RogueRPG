using System.Collections.Generic;
using Hsenl.numeric;
using Hsenl.timeline;

namespace Hsenl {
    public abstract class TpHarm<T> : TpInfo<T> where T : TpHarmInfo {
        protected readonly List<Numerator> numerators = new(2);
        protected Harmable harmable;
        protected ProcedureLine procedureLine;

        protected override void OnNodeOpen() {
            var owner = this.manager.Owner;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);

                    this.harmable = owner?.GetComponent<Harmable>();
                    this.procedureLine = owner?.GetComponent<ProcedureLine>();
                    break;
                }
            }
        }

        protected void Harm(Hurtable hurtable, DamageFormulaInfo damageFormulaInfo, float damageRate = 1f) {
            var dmg = Num.Empty();
            for (int i = 0, len = damageFormulaInfo.DamageFormulas.Count; i < len; i++) {
                var formulaInfo = damageFormulaInfo.DamageFormulas[i];
                var d = GameAlgorithm.MergeCalculateNumeric(this.numerators, formulaInfo.Type);
                d *= formulaInfo.Pct;
                d += formulaInfo.Fix;
                dmg += d;
            }

            var damageForm = new PliDamageArbitramentForm {
                harm = this.harmable,
                hurt = hurtable,
                source = this.manager.Bodied,
                damageType = damageFormulaInfo.DamageType,
                damage = dmg * damageRate,
                astun = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Astun),
            };

            this.OnHarm(ref damageForm);
        }

        protected virtual void OnHarm(ref PliDamageArbitramentForm damageArbitramentForm) {
            this.procedureLine.StartLineAsync(damageArbitramentForm).Coroutine();
        }
    }
}