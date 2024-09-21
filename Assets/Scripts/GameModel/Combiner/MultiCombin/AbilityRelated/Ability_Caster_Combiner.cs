using System;

namespace Hsenl.MultiCombiner {
    public class Ability_Caster_Combiner : MultiCombiner<Ability, Caster> {
        protected override void OnCombin(Ability arg1, Caster arg2) {
            arg2.onEnter += this.EnqueueAction<Action>(arg1.OnAbilityCastStart);
            arg2.onLeave += this.EnqueueAction<Action>(arg1.OnAbilityCastEnd);
        }

        protected override void OnDecombin(Ability arg1, Caster arg2) {
            arg2.onEnter -= this.DequeueAction<Action>();
            arg2.onLeave -= this.DequeueAction<Action>();
        }
    }
}