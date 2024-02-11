using System.Collections.Generic;
using Hsenl.numeric;

namespace Hsenl {
    public abstract class TsStatusHarm<T> : TsHarm<T> where T : timeline.TsHarmInfo {
        protected Numerator inflictorNumerator;
        protected Numerator beinflictorNumerator;
        protected Numerator statusNumerator;

        protected override void OnNodeReset() {
            switch (this.manager.Bodied) {
                case Status status: {
                    this.harmable = status.inflictor.GetComponent<Harmable>();
                    this.procedureLine = status.AttachedBodied.GetComponent<ProcedureLine>();

                    // 状态伤害计算规则
                    // 施加者伤害 + 状态本身伤害 = 伤害数值
                    // 施加者症状强化 - 被施加者症状抵抗 = 伤害强化比例
                    // 状态本身的硬直 = 伤害硬直 (状态造成的伤害硬直, 不会受施加者影响)
                    this.inflictorNumerator = status.inflictor.GetComponent<Numerator>();
                    this.beinflictorNumerator = status.AttachedBodied.GetComponent<Numerator>();
                    this.statusNumerator = status.GetComponent<Numerator>();

                    break;
                }
            }
        }

        protected override void Harm(Hurtable hurtable, DamageFormulaInfo damageFormulaInfo, float damageRate = 1) {
            var sit = this.inflictorNumerator.GetValue(NumericType.Sit);
            var srt = this.beinflictorNumerator.GetValue(NumericType.Srt);
            var intensify = sit - srt;
            var astun = this.statusNumerator.GetValue(NumericType.Astun);

            var dmg = Num.Empty();
            for (int j = 0, len = damageFormulaInfo.DamageFormulas.Count; j < len; j++) {
                var formulaInfo = damageFormulaInfo.DamageFormulas[j];
                var d = GameAlgorithm.MergeCalculateNumeric(this.inflictorNumerator, this.statusNumerator, formulaInfo.Type);
                d *= formulaInfo.Pct;
                d += formulaInfo.Fix;
                dmg += d;
            }

            dmg -= dmg * intensify;
            if (dmg <= 0) {
                dmg = 1;
            }

            var damageForm = new PliDamageArbitramentForm {
                harm = this.harmable,
                hurt = hurtable,
                source = this.manager.AttachedBodied,
                damageType = damageFormulaInfo.DamageType,
                damage = dmg * damageRate,
                astun = astun,
            };

            this.OnHarm(ref damageForm);
        }
    }
}