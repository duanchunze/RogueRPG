using System;

namespace Hsenl.MultiCombiner {
    public class AbilityCasterCombiner : MultiCombiner<Ability, Caster> {
        protected override void OnCombin(Ability arg1, Caster arg2) {
            arg2.onEnter += this.EnqueueAction<Action>(arg1.OnAbilityEnter);
            arg2.onLeave += this.EnqueueAction<Action>(arg1.OnAbilityLeave);
        }

        protected override void OnDecombin(Ability arg1, Caster arg2) {
            arg2.onEnter -= this.DequeueAction<Action>();
            arg2.onLeave -= this.DequeueAction<Action>();
        }
    }
}