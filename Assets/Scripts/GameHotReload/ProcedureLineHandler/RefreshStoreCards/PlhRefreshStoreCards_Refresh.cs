// namespace Hsenl {
//     [ProcedureLineHandlerPriority(PliRefreshStoreCardsPriority.Refresh)]
//     public class PlhRefreshStoreCards_Refresh : AProcedureLineHandler<PliRefreshStoreCardsForm> {
//         protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliRefreshStoreCardsForm item) {
//             CardStore.Instance.RefreshCards();
//             return ProcedureLineHandleResult.Success;
//         }
//     }
// }