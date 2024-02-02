using System;

namespace Hsenl.CrossCombiner {
    public class Ability_AbilityBar_Combiner : CrossCombiner<Ability, AbilityBar> {
        protected override void OnCombin(Ability arg1, AbilityBar arg2) {
            arg1.onAbilityEnter += this.EnqueueAction<Action>(() => {
                arg2.currentCastingAbilities.Add(arg1);
            });
            arg1.onAbilityLeave += this.EnqueueAction<Action>(() => {
                arg2.currentCastingAbilities.Remove(arg1);
            });
        }

        protected override void OnDecombin(Ability arg1, AbilityBar arg2) {
            arg1.onAbilityEnter -= this.DequeueAction<Action>();
            arg1.onAbilityLeave -= this.DequeueAction<Action>();
        }
    }
}