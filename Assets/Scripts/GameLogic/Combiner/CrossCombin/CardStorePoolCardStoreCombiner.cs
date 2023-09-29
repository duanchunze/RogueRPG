using System;

namespace Hsenl.CrossCombiner {
    public class CardStorePoolCardStoreCombiner : CrossCombiner<CardStorePool, CardStore> {
        protected override void OnCombin(CardStorePool arg1, CardStore arg2) {
            arg1.onCardPutin += this.EnqueueAction(new Action<Card>(arg2.OnAddCardToPool));
            arg1.onCardTakeout += this.EnqueueAction(new Action<Card>(arg2.OnRemoveCardFromPool));
        }

        protected override void OnDecombin(CardStorePool arg1, CardStore arg2) {
            arg1.onCardPutin -= this.DequeueAction<Action<Card>>();
            arg1.onCardTakeout -= this.DequeueAction<Action<Card>>();
        }
    }
}