using System;
using System.Collections.Generic;
using Hsenl.EventType;

namespace Hsenl {
    [Serializable]
    public abstract class CardResidence<TSlot> : Bodied, ICardResidence where TSlot : CardSlot {
        public Action<TSlot, Card> onPutinCard;
        public Action<TSlot, Card> onTakeoutCard;

        public Action onChanged;

        public Card GetCard(string cardName) {
            foreach (var card in this.FindScopesInBodied<Card>()) {
                if (card.Name == cardName) {
                    return card;
                }
            }

            return null;
        }

        // 评估, 并返回可以放入的槽
        public virtual TSlot PutinCardEvaluate(Card card, TSlot slot = null) {
            // 如果指定了槽, 就放指定槽
            if (slot != null) {
                if (slot.PutinCardEvaluate(card))
                    return slot;

                return null;
            }

            // 没有指定槽, 就遍历所有槽, 哪个能放就放哪个
            foreach (var cardSlot in this.FindScopesInBodied<TSlot>()) {
                if (cardSlot.PutinCardEvaluate(card))
                    return cardSlot;
            }

            return null;
        }

        public virtual bool PutinCard(Card card, TSlot slot = null) {
            // 如果指定了槽, 就放指定槽
            if (slot != null) {
                return slot.PutinCard(card);
            }

            // 没有指定槽, 就遍历所有槽, 哪个能放就放哪个
            foreach (var cardSlot in this.FindScopesInBodied<TSlot>()) {
                if (cardSlot.PutinCard(card))
                    return true;
            }

            return false;
        }

        public virtual void OnPutinCard(TSlot slot, Card card) {
            try {
                this.onPutinCard?.Invoke(slot, card);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.OnChanged();
        }

        public virtual void OnTakeoutCard(TSlot slot, Card card) {
            try {
                this.onTakeoutCard?.Invoke(slot, card);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.OnChanged();
        }

        public virtual void OnChanged() {
            SourceEventStation.OnCardResidenceChanged(this);
            try {
                this.onChanged?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}