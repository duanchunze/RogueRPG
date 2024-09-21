// namespace Hsenl {
//     [ProcedureLineHandlerPriority(PliBuyCardPriority.Pay)]
//     public class PlhBuyCard_Pay : AProcedureLineHandler<PliBuyCardForm> {
//         protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item) {
//             if (item.target == null)
//                 return ProcedureLineHandleResult.Success;
//
//             if (!item.free) {
//                 // 付钱
//                 if (GameManager.Instance.gold < item.target.Config.Price) {
//                     // 钱不够
//                     return ProcedureLineHandleResult.Break;
//                 }
//
//                 GameManager.Instance.RemoveGold(item.target.Config.Price);
//             }
//
//             return ProcedureLineHandleResult.Success;
//         }
//     }
// }