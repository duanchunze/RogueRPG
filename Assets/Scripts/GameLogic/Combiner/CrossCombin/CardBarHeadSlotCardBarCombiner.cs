﻿using System;

namespace Hsenl.CrossCombiner {
    public class CardBarHeadSlotCardBarCombiner : CrossCombiner<CardBarHeadSlot, CardBar> {
        protected override void OnCombin(CardBarHeadSlot arg1, CardBar arg2) {
            arg1.onCardPutin += this.EnqueueAction(new Action<Card>(card => {
                arg2.OnPutinCard(arg1, card);
            }));
            arg1.onCardTakeout += this.EnqueueAction(new Action<Card>(card => {
                arg2.OnTakeoutCard(arg1, card);
            }));
        }

        protected override void OnDecombin(CardBarHeadSlot arg1, CardBar arg2) {
            arg1.onCardPutin -= this.DequeueAction<Action<Card>>();
            arg1.onCardTakeout -= this.DequeueAction<Action<Card>>();
        }
    }
}