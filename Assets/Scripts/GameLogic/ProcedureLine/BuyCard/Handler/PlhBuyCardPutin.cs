namespace Hsenl {
    [ProcedureLineHandlerPriority(PliBuyCardPriority.Putin)]
    public class PlhBuyCardPutin : AProcedureLineHandler<PliBuyCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item) {
            // 把卡牌放进去
            if (item.synthesis) {
                foreach (var destroyCard in item.synthesisDestroyCards) {
                    Object.Destroy(destroyCard.Entity);
                }

                foreach (var achievement in item.synthesisAchievements) {
                    var card = CardFactory.Create(achievement);
                    switch (item.buyDestination) {
                        case CardBar cardBar: {
                            var ret = cardBar.PutinCard(card);
                            if (!ret) {
                                cardBar.GetHolder().FindSubstaintiveInChildren<CardBackpack>().PutinCard(card);
                            }

                            break;
                        }
                        case CardBackpack cardBackpack: {
                            var ret = cardBackpack.PutinCard(card);
                            if (!ret) {
                                cardBackpack.GetHolder().FindSubstaintiveInChildren<CardBar>().PutinCard(card);
                            }

                            break;
                        }
                    }
                }

                // 合成后, 再重新走一遍流程, 把card = null, 意思就是纯以合成为目的再重走一遍(todo 这里为了省事这么做, 后续应该改掉)
                item.buyOriginal = null;
                item.slot = null;
                item.card = null;
                item.synthesis = false;
                item.synthesisDestroyCards.Clear();
                item.synthesisAchievements.Clear();
                procedureLine.StartLine(ref item);
            }
            else {
                if (item.card == null)
                    return ProcedureLineHandleResult.Success;

                item.slot?.PutinCard(item.card);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}