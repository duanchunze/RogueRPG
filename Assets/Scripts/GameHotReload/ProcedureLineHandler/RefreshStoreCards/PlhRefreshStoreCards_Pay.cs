// namespace Hsenl {
//     [ProcedureLineHandlerPriority(PliRefreshStoreCardsPriority.Pay)]
//     public class PlhRefreshStoreCards_Pay : AProcedureLineHandler<PliRefreshStoreCardsForm> {
//         protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliRefreshStoreCardsForm item) {
//             if (GameManager.Instance.gold < item.price) {
//                 return ProcedureLineHandleResult.Break;
//             }
//
//             GameManager.Instance.RemoveGold(item.price);
//             return ProcedureLineHandleResult.Success;
//         }
//     }
// }