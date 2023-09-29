// using Hsenl.PlItem;
//
// namespace Hsenl.MultiCombiner {
//     public class HarmableProcedureLineCombiner : SingleCombiner<Harmable, ProcedureLine> {
//         protected override void OnCombin(Harmable arg1, ProcedureLine arg2) {
//             arg1.harmInvoke = hurtable => {
//                 HarmArbitramentForm form = new() {
//                     harm = arg1,
//                     hurt = hurtable,
//                 };
//
//                 arg2.StartLine(ref form);
//             };
//         }
//
//         protected override void OnDecombin(Harmable arg1, ProcedureLine arg2) {
//             arg1.harmInvoke = null;
//         }
//     }
// }