using System.Collections.Generic;
using Hsenl.numeric;

namespace Hsenl {
    public abstract class TsHarm<T> : TsInfo<T> where T : timeline.TsHarmInfo {
        protected List<Numerator> numerators;
        protected Harmable harmable;
        protected ProcedureLine procedureLine;

        // 技能和状态伤害的方式不同, 因为技能我们是可以保证holder一直存在的, 但状态就不行, 可能上完状态后, 这个人就死了, 所以状态是先把伤害存下来, 再使用
        protected override void OnNodeOpen() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this.numerators ??= new(2);
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = ability.AttachedBodied?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    this.harmable = ability.AttachedBodied?.GetComponent<Harmable>();
                    this.procedureLine = ability.AttachedBodied?.GetComponent<ProcedureLine>();
                    break;
                }
            }
        }


        protected virtual void Harm(Hurtable hurtable, DamageFormulaInfo damageFormulaInfo, float damageRate = 1f) {
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
            this.procedureLine.StartLineAsync(damageArbitramentForm).Tail();
        }
    }
}