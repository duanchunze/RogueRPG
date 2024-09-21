// using System;
//
// namespace Hsenl.CrossCombiner {
//     // 这是一个例子, 展示如何让combiner支持热重载, 因为必要性不大, 所以当前项目没有把combiner写为热重载版本
//     [ShadowFunction]
//     public partial class AAExample : CrossCombiner<Ability, AbilitesBar> {
//         protected override void OnCombin(Ability arg1, AbilitesBar arg2) {
//             arg1.onAbilityCastStart += this.EnqueueAction<Action>(() => { this.OnAbilityCastEnter(arg1, arg2); });
//             arg1.onAbilityCastEnd += this.EnqueueAction<Action>(() => { this.OnAbilityCastEnd(arg1, arg2); });
//         }
//
//         protected override void OnDecombin(Ability arg1, AbilitesBar arg2) {
//             arg1.onAbilityCastStart -= this.DequeueAction<Action>();
//             arg1.onAbilityCastEnd -= this.DequeueAction<Action>();
//         }
//
//         [ShadowFunction]
//         private void OnAbilityCastEnter(Ability ability, AbilitesBar abilitesBar) {
//             this.OnAbilityCastEnterShadow(ability, abilitesBar);
//         }
//
//         [ShadowFunction]
//         private void OnAbilityCastEnd(Ability ability, AbilitesBar abilitesBar) {
//             this.OnAbilityCastEndShadow(ability, abilitesBar);
//         }
//     }
// }