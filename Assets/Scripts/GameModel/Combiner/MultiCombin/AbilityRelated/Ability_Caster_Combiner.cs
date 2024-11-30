using System;

namespace Hsenl.MultiCombiner {
    public class Ability_Caster_Combiner : MultiCombiner<Ability, Caster> {
        protected override void OnCombin(Ability arg1, Caster arg2) {
            arg2.onCastStart += this.EnqueueAction<Action>(arg1.OnAbilityCastStart);
            arg2.onCastEnd += this.EnqueueAction<Action<CasterEndDetails>>(_ => { arg1.OnAbilityCastEnd(); });
        }

        protected override void OnDecombin(Ability arg1, Caster arg2) {
            arg2.onCastStart -= this.DequeueAction<Action>();
            arg2.onCastEnd -= this.DequeueAction<Action<CasterEndDetails>>();
        }
    }
}