using System;

namespace Hsenl.CrossCombiner {
    public class CardStoreSlotCardStoreCombiner : CrossCombiner<CardStoreSlot, CardStore> {
        protected override void OnCombin(CardStoreSlot arg1, CardStore arg2) {
            arg1.onCardPutin += this.EnqueueAction(new Action<Card>(card => {
                arg2.OnPutinCard(arg1, card);
            }));
            arg1.onCardTakeout += this.EnqueueAction(new Action<Card>(card => {
                arg2.OnTakeoutCard(arg1, card);
            }));
        }

        protected override void OnDecombin(CardStoreSlot arg1, CardStore arg2) {
            arg1.onCardPutin -= this.DequeueAction<Action<Card>>();
            arg1.onCardTakeout -= this.DequeueAction<Action<Card>>();
        }
    }
}