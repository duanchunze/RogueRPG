// using Hsenl.PlItem;
//
// namespace Hsenl.MultiCombiner {
//     public class HurtableProcedureLineCombiner : SingleCombiner<Hurtable, ProcedureLine> {
//         protected override void OnCombin(Hurtable arg1, ProcedureLine arg2) {
//             arg1.hurtInvoke = harmable => {
//                 HurtForm form = new() {
//                 };
//
//                 arg2.StartLine(ref form);
//             };
//         }
//
//         protected override void OnDecombin(Hurtable arg1, ProcedureLine arg2) {
//             
//         }
//     }
// }