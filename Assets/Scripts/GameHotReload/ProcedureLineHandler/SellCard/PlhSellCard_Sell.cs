namespace Hsenl {
    [ProcedureLineHandlerPriority(PliSellCardPriority.Sell)]
    public class PlhSellCard_Sell : AProcedureLineHandler<PliSellCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliSellCardForm item, object userToken) {
            Object.Destroy(item.card.Entity);
            return ProcedureLineHandleResult.Success;
        }
    }
}