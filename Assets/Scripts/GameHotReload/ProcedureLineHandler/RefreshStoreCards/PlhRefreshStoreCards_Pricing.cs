// namespace Hsenl {
//     [ProcedureLineHandlerPriority(PliRefreshStoreCardsPriority.Pricing)]
//     public class PlhRefreshStoreCards_Pricing : AProcedureLineHandler<PliRefreshStoreCardsForm> {
//         protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliRefreshStoreCardsForm item) {
//             if (CardStore.Instance.refreshFreeTime > 0) {
//                 item.price = 0;
//                 CardStore.Instance.refreshFreeTime--;
//             }
//             else {
//                 item.price = CardStore.Instance.refreshCost;
//             }
//
//             return ProcedureLineHandleResult.Success;
//         }
//     }
// }