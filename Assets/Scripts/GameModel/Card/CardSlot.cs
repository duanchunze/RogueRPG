using System;

namespace Hsenl {
    [Serializable]
    public abstract class CardSlot : Substantive {
        public Bitlist confineCardType = new();

        public Action<Card> onCardPutin;
        public Action<Card> onCardTakeout;

        public Card StayCard => this.FindSubstaintiveInChildren<Card>();

        public virtual bool PutinCardEvaluate(Card card) {
            if (!this.confineCardType.ContainsAny(card.cardType)) {
                return false;
            }

            return this.StayCard == null;
        }

        public virtual bool PutinCard(Card card) {
            if (this.StayCard != null)
                return false;

            if (!this.confineCardType.ContainsAny(card.cardType)) {
                return false;
            }

            card.SetParent(this.Entity);
            return true;
        }

        public virtual Card TakeoutCard() {
            var card = this.StayCard;
            if (card == null)
                return null;

            card.SetParent(null);
            return card;
        }

        protected override void OnChildSubstantiveAdd(Substantive childSubs) {
            if (childSubs is not Card card)
                return;

            this.OnCardPutin(card);
        }

        protected override void OnChildSubstantiveRemove(Substantive childSubs) {
            if (childSubs is not Card card)
                return;

            this.OnCardTakeout(card);
        }

        protected virtual void OnCardPutin(Card card) {
            try {
                this.onCardPutin?.Invoke(card);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected virtual void OnCardTakeout(Card card) {
            try {
                this.onCardTakeout?.Invoke(card);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}