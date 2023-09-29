using System;
using UnityEngine;

namespace Hsenl.CrossCombiner {
    public class CardBarAssistSlotCardBarCombiner : CrossCombiner<CardBarAssistSlot, CardBar> {
        protected override void OnCombin(CardBarAssistSlot arg1, CardBar arg2) {
            arg1.onCardPutin += this.EnqueueAction(new Action<Card>(card => { arg2.OnPutinCard(arg1, card); }));
            arg1.onCardTakeout += this.EnqueueAction(new Action<Card>(card => { arg2.OnTakeoutCard(arg1, card); }));
        }

        protected override void OnDecombin(CardBarAssistSlot arg1, CardBar arg2) {
            arg1.onCardPutin -= this.DequeueAction<Action<Card>>();
            arg1.onCardTakeout -= this.DequeueAction<Action<Card>>();
        }
    }
}