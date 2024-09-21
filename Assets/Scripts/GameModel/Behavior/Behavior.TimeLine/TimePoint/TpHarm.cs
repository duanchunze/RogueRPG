using System.Collections.Generic;
using Hsenl.numeric;
using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    public abstract class TpHarm<T> : TpInfo<T>, IHarmInfo where T : TpHarmInfo {
        protected readonly List<Numerator> numerators = new(2);
        protected Harmable harmable;
        protected readonly List<ProcedureLine> procedureLines = new(2);
        
        [MemoryPackIgnore]
        public object HarmInfo => this.info;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = owner?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);

                    this.harmable = owner?.GetComponent<Harmable>();
                    
                    this.procedureLines.Clear();
                    var pl = owner?.GetComponent<ProcedureLine>();
                    if (pl != null) this.procedureLines.Add(pl);
                    pl = ability.GetComponent<ProcedureLine>();
                    if (pl != null) this.procedureLines.Add(pl);
                    break;
                }
            }
        }

        protected void Harm(Hurtable hurtable, DamageFormulaInfo damageFormulaInfo, float damageRate = 1f) {
            // var dmg = Num.Empty();
            // for (int i = 0, len = damageFormulaInfo.DamageFormulas.Count; i < len; i++) {
            //     var formulaInfo = damageFormulaInfo.DamageFormulas[i];
            //     var d = GameAlgorithm.MergeCalculateNumeric(this.numerators, formulaInfo.Type);
            //     d *= formulaInfo.Pct;
            //     d += formulaInfo.Fix;
            //     dmg += d;
            // }
            //
            // var damageForm = new PliHarmForm {
            //     harmable = this.harmable,
            //     hurtable = hurtable,
            //     source = this.manager.Bodied,
            //     damageType = damageFormulaInfo.DamageType,
            //     damage = dmg * damageRate,
            //     astun = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Astun),
            // };
            //
            // this.manager.Blackboard.TryGetData(BlackboardKey.HitSound.ToString(), out damageForm.hitsound);
            // this.manager.Blackboard.TryGetData(BlackboardKey.HitFx.ToString(), out damageForm.hitfx);
            
            this.manager.Blackboard.TryGetData(BlackboardKey.HitSound.ToString(), out string hitsound);
            this.manager.Blackboard.TryGetData(BlackboardKey.HitFx.ToString(), out string hitfx);
            Shortcut.TakeDamage(this.harmable, hurtable, this.manager.Bodied, damageFormulaInfo, this.procedureLines, hitfx, hitsound, damageRate);
        }
    }
}