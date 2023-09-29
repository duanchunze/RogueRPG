namespace Hsenl {
    [ProcedureLineHandlerPriority(PliBuyCardPriority.Evaluate)]
    public class PlhBuyCardPutinEvaluate : AProcedureLineHandler<PliBuyCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item) {
            if (item.card == null)
                return ProcedureLineHandleResult.Success;

            // 判断是否能放进去, 并找到那个能放进去的槽
            if (!item.synthesis) {
                var dst = item.buyDestination;
                switch (dst) {
                    case CardBar cardBar: {
                        // 先放卡牌栏, 如果放入失败, 再尝试放卡牌背包
                        item.slot = cardBar.PutinCardEvaluate(item.card, item.slot as CardBarSlot);
                        if (item.slot == null) {
                            var cardBackpack = cardBar.GetHolder().FindSubstaintiveInChildren<CardBackpack>();
                            item.slot = cardBackpack.PutinCardEvaluate(item.card, item.slot as CardBackpackSlot);
                            if (item.slot == null) {
                                // 都失败直接跳出, 购买失败
                                return ProcedureLineHandleResult.Break;
                            }
                        }

                        break;
                    }

                    case CardBackpack cardBackpack: {
                        item.slot = cardBackpack.PutinCardEvaluate(item.card, item.slot as CardBackpackSlot);
                        if (item.slot == null) {
                            return ProcedureLineHandleResult.Break;
                        }

                        break;
                    }
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}