using System;

namespace Hsenl.CrossCombiner {
    public class AbilityBarProcedureLineCombiner : CrossCombiner<AbilityBar, ProcedureLine> {
        protected override void OnCombin(AbilityBar arg1, ProcedureLine arg2) {
            arg1.onAbilityAdd += this.EnqueueAction<Action<Ability>>(abi => {
                arg2.StartLineAsync(new PliAbilityChangedForm() { ability = abi, bar = arg1, changeType = 0 }).Coroutine();
            });

            arg1.onAbilityRemove += this.EnqueueAction<Action<Ability>>(abi => {
                arg2.StartLineAsync(new PliAbilityChangedForm() { ability = abi, bar = arg1, changeType = 1 }).Coroutine();
            });
        }

        protected override void OnDecombin(AbilityBar arg1, ProcedureLine arg2) {
            arg1.onAbilityAdd -= this.DequeueAction<Action<Ability>>();
            arg1.onAbilityRemove -= this.DequeueAction<Action<Ability>>();
        }
    }
}