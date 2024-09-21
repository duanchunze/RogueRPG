using System.Collections.Generic;

namespace Hsenl {
    public class PliBuyCardForm {
        public Bodied buyOriginal;
        public Bodied buyDestination;
        // public CardSlot slot;
        public Bodied target; // 要购买的东西

        public bool free;

        public bool synthesis; // 可以合成
        public List<Bodied> synthesisDestroys;
        public List<string> synthesisAchievements;
    }
}