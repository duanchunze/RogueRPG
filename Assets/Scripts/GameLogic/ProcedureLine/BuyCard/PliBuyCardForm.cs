using System.Collections.Generic;

namespace Hsenl {
    public class PliBuyCardForm {
        public ICardResidence buyOriginal;
        public ICardResidence buyDestination;
        public CardSlot slot;
        public Card card;

        public bool free;

        public bool synthesis; // 可以合成
        public List<Card> synthesisDestroyCards;
        public List<string> synthesisAchievements;
    }
}