// namespace Hsenl {
//     [ProcedureLineHandlerPriority(PliSellCardPriority.Pricing)]
//     public class PlhSellCard_Pricing : AProcedureLineHandler<PliSellCardForm> {
//         protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliSellCardForm item) {
//             item.sellPrice = item.card.Config.Price;
//             
//             return ProcedureLineHandleResult.Success;
//         }
//     }
// }