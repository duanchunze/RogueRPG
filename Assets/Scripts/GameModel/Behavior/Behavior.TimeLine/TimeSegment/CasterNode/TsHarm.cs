using System.Collections.Generic;
using Hsenl.numeric;
using MemoryPack;

namespace Hsenl {
    public abstract class TsHarm<T> : TsInfo<T>, IHarmInfo where T : timeline.TsHarmInfo {
        protected List<Numerator> numerators;
        protected Harmable harmable;
        protected List<ProcedureLine> procedureLines;

        [MemoryPackIgnore]
        public object HarmInfo => this.info;

        // 技能和状态伤害的方式不同, 因为技能我们是可以保证holder一直存在的, 但状态就不行, 可能上完状态后, 这个人就死了, 所以状态是先把伤害存下来, 再使用
        protected override void OnEnable() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    this.numerators ??= new(2);
                    this.numerators.Clear();
                    var numerator = ability.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);
                    numerator = ability.MainBodied?.GetComponent<Numerator>();
                    if (numerator != null) this.numerators.Add(numerator);

                    this.harmable = ability.MainBodied?.GetComponent<Harmable>();

                    this.procedureLines ??= new(2);
                    this.procedureLines.Clear();
                    var pl = ability.MainBodied?.GetComponent<ProcedureLine>();
                    if (pl != null) this.procedureLines.Add(pl);
                    pl = ability.GetComponent<ProcedureLine>();
                    if (pl != null) this.procedureLines.Add(pl);
                    break;
                }
            }
        }


        protected void Harm(Hurtable hurtable, DamageFormulaInfo damageFormulaInfo, float damageRate = 1f) {
            this.manager.Blackboard.TryGetData(BlackboardKey.HitSound.ToString(), out string hitsound);
            this.manager.Blackboard.TryGetData(BlackboardKey.HitFx.ToString(), out string hitfx);
            Shortcut.TakeDamage(this.harmable, hurtable, this.manager.Bodied, damageFormulaInfo, this.procedureLines, hitfx, hitsound, damageRate);
        }
    }
}