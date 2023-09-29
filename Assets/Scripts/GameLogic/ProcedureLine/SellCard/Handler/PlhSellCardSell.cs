namespace Hsenl {
    [ProcedureLineHandlerPriority(PliSellCardPriority.Sell)]
    public class PlhSellCardSell : AProcedureLineHandler<PliSellCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliSellCardForm item) {
            Object.Destroy(item.card.Entity);
            return ProcedureLineHandleResult.Success;
        }
    }
}