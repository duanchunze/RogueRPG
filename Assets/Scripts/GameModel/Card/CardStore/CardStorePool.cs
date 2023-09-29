using System;

namespace Hsenl {
    [Serializable]
    public class CardStorePool : Substantive {
        public Action<Card> onCardPutin;
        public Action<Card> onCardTakeout;

        protected override void OnChildSubstantiveAdd(Substantive childSubs) {
            if (childSubs is not Card card)
                return;
            
            this.onCardPutin?.Invoke(card);
        }

        protected override void OnChildSubstantiveRemove(Substantive childSubs) {
            if (childSubs is not Card card)
                return;
            
            this.onCardTakeout?.Invoke(card);
        }
    }
}