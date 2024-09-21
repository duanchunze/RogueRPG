using System;

namespace Hsenl.CrossCombiner {
    public class AbilityBar_ProcedureLine_Combiner : CrossCombiner<AbilitesBar, ProcedureLine> {
        protected override void OnCombin(AbilitesBar arg1, ProcedureLine arg2) {
            arg1.onAbilityAdd += this.EnqueueAction<Action<Ability>>(abi => {
                arg2.StartLineAsync(new PliAbilityChangedForm() { ability = abi, bar = arg1, changeType = 0 }).Tail();
            });

            arg1.onAbilityRemove += this.EnqueueAction<Action<Ability>>(abi => {
                arg2.StartLineAsync(new PliAbilityChangedForm() { ability = abi, bar = arg1, changeType = 1 }).Tail();
            });
        }

        protected override void OnDecombin(AbilitesBar arg1, ProcedureLine arg2) {
            arg1.onAbilityAdd -= this.DequeueAction<Action<Ability>>();
            arg1.onAbilityRemove -= this.DequeueAction<Action<Ability>>();
        }
    }
}