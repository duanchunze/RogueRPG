using System;

namespace Hsenl.CrossCombiner {
    public class Ability_AbilityBar_Combiner : CrossCombiner<Ability, AbilitesBar> {
        protected override void OnCombin(Ability arg1, AbilitesBar arg2) {
            arg1.onAbilityCastStart += this.EnqueueAction<Action>(() => {
                arg2.currentCastingAbilities.Add(arg1);
            });
            arg1.onAbilityCastEnd += this.EnqueueAction<Action>(() => {
                arg2.currentCastingAbilities.Remove(arg1);
            });
        }

        protected override void OnDecombin(Ability arg1, AbilitesBar arg2) {
            arg1.onAbilityCastStart -= this.DequeueAction<Action>();
            arg1.onAbilityCastEnd -= this.DequeueAction<Action>();
        }
    }
}