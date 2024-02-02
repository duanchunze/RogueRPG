using System;

namespace Hsenl {
    [Serializable]
    public class CardStorePool : Bodied {
        public Action<Card> onCardPutin;
        public Action<Card> onCardTakeout;
        
        protected override void OnChildScopeAdd(Scope child) {
            if (child is not Card card)
                return;
            
            this.onCardPutin?.Invoke(card);
        }

        protected override void OnChildScopeRemove(Scope child) {
            if (child is not Card card)
                return;
            
            this.onCardTakeout?.Invoke(card);
        }
    }
}