using System.Collections.Generic;

namespace Hsenl {
    public class CardSynthesisInfo {
        public Card[] members;
        public string achievement;
    }

    // 游戏卡牌合成公式
    public static class GameCardSynthesisFormula {
        public static bool Match(List<Card> cards, out List<CardSynthesisInfo> synthesisInfos) {
            var results = new List<CardSynthesisInfo>();
            using var cache = ListComponent<Card>.Create();
            foreach (var synthesisConfig in Tables.Instance.TbCardSynthesisConfig.DataList) {
                cache.Clear();
                // 遍历合成公式里的成员名单, 逐一比对, 如果全部都找到了, 则符合合成条件
                foreach (var synthesisMember in synthesisConfig.SynthesisMembers) {
                    var index = cards.FindIndex(s => s.Name == synthesisMember);
                    if (index == -1) {
                        goto NOMATCH;
                    }

                    cache.Add(cards[index]);
                    cards.RemoveAt(index);
                }

                var synthesisInfo = new CardSynthesisInfo {
                    members = cache.ToArray(),
                    achievement = synthesisConfig.SynthesisAchievement
                };

                results.Add(synthesisInfo);

                continue;

                NOMATCH:
                foreach (var card in cache) {
                    cards.Add(card);
                }
            }

            synthesisInfos = results;
            return results.Count != 0;
        }
    }
}