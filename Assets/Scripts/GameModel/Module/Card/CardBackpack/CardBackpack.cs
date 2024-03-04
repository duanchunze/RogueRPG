using System;
using System.Collections.Generic;
using Hsenl.EventType;

namespace Hsenl {
    [Serializable]
    public class CardBackpack : CardResidence<CardBackpackSlot> {
        public int capacity;

        protected override void OnAwake() {
            for (var i = 0; i < this.capacity; i++) {
                var entity = Entity.Create("CardBackpackSlot");
                var slot = entity.AddComponent<CardBackpackSlot>();
                slot.confineCardType.Add(CardType.Ability);
                slot.confineCardType.Add(CardType.AbilityAssist);
                slot.SetParent(this.Entity);
            }

            this.OnChanged();
        }

        public override void OnPutinCard(CardBackpackSlot slot, Card card) {
            card.Reset();
            base.OnPutinCard(slot, card);
        }

        public Card[] GetCards() {
            using var list = ListComponent<Card>.Create();
            foreach (var child in this.Entity.ForeachChildren()) {
                var card = child.GetComponent<CardBackpackSlot>()?.StayCard;
                if (card == null)
                    continue;

                list.Add(card);
            }

            return list.ToArray();
        }
    }
}