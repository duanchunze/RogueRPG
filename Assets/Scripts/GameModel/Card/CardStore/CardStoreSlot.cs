using System;

namespace Hsenl {
    [Serializable]
    public class CardStoreSlot : CardSlot {
        public bool freeze;

        protected override void OnCardTakeout(Card card) {
            if (this.freeze) {
                this.freeze = false;
            }

            base.OnCardTakeout(card);
        }
    }
}