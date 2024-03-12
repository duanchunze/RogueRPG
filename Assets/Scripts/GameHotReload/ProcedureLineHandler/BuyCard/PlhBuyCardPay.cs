namespace Hsenl {
    [ProcedureLineHandlerPriority(PliBuyCardPriority.Pay)]
    public class PlhBuyCardPay : AProcedureLineHandler<PliBuyCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item) {
            if (item.card == null)
                return ProcedureLineHandleResult.Success;

            if (!item.free) {
                // 付钱
                if (GameManager.Instance.gold < item.card.Config.Price) {
                    // 钱不够
                    return ProcedureLineHandleResult.Break;
                }

                GameManager.Instance.RemoveGold(item.card.Config.Price);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}