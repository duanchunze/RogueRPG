namespace Hsenl {
    [ProcedureLineHandlerPriority(PliSellCardPriority.Pricing)]
    public class PlhSellCard_Pricing : AProcedureLineHandler<PliSellCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliSellCardForm item, object userToken) {
            item.sellPrice = 0;
            return ProcedureLineHandleResult.Success;
        }
    }
}