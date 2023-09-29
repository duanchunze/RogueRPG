using System;
using System.Collections.Generic;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliBuyCardPriority.Synthesis)]
    public class PlhBuyCardSynthesis : AProcedureLineHandler<PliBuyCardForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliBuyCardForm item) {
            var dst = item.buyDestination;
            switch (dst) {
                case CardBar cardBar: {
                    // 判断是否符合合成条件
                    if (this.CardSynthesisEvaluate(ref item, cardBar, cardBar.GetHolder().FindSubstaintiveInChildren<CardBackpack>(),
                            out var synthesisInfos)) { }

                    break;
                }

                case CardBackpack cardBackpack: {
                    var cardBar = cardBackpack.GetHolder().FindSubstaintiveInChildren<CardBar>();
                    // 判断是否符合合成条件
                    if (this.CardSynthesisEvaluate(ref item, cardBar, cardBackpack,
                            out var synthesisInfos)) { }

                    break;
                }
                
                default:
                    throw new Exception("buy card dst residence must card bar of card backpack"); // 购买卡牌流程要求目标仓库不能是其他
            }

            return ProcedureLineHandleResult.Success;
        }

        private bool CardSynthesisEvaluate(ref PliBuyCardForm item, CardBar cardBar, CardBackpack cardBackpack, out List<CardSynthesisInfo> synthesisInfos) {
            synthesisInfos = null;
            var cards1 = cardBar.GetHeadCards();
            var cards2 = cardBackpack.GetCards();
            using var matchCards = ListComponent<Card>.Create();
            foreach (var card in cards1) {
                matchCards.Add(card);
            }

            foreach (var card in cards2) {
                matchCards.Add(card);
            }

            if (item.card != null)
                matchCards.Add(item.card);

            if (GameCardSynthesisFormula.Match(matchCards, out var result)) {
                item.synthesis = true;
                item.synthesisDestroyCards = new();
                foreach (var synthesisInfo in result) {
                    item.synthesisDestroyCards.AddRange(synthesisInfo.members);
                }

                item.synthesisAchievements = new();
                foreach (var synthesisInfo in result) {
                    item.synthesisAchievements.Add(synthesisInfo.achievement);
                }

                return true;
            }

            return false;
        }
    }
}