using System;
using Hsenl.EventType;

namespace Hsenl {
    public class EventMoveCard : AEventSync<MoveCard> {
        protected override void Handle(MoveCard arg) {
            var cardRaw = EventSystem.GetInstance<Card>(arg.cardInstanceId);
            if (cardRaw == null)
                return;

            var card = cardRaw;
            if (arg.copyNew) {
                card = Object.InstantiateWithUnity(cardRaw.Entity).GetComponent<Card>();
            }

            // 获取卡牌的起源地
            var cardOriginal = cardRaw.StaySlot?.GetSubstantiveInParent<ICardResidence>();
            var destSlot = EventSystem.GetInstance<CardSlot>(arg.slotInstanceId);

            switch (cardOriginal) {
                case null: {
                    if (arg.destination == typeof(CardStore)) {
                        // 无效
                    }
                    else if (arg.destination == typeof(CardBar)) {
                        // 放到卡牌栏里
                        // var dstResidence = GameManager.Instance.MainMan.FindSubstaintiveInChildren<CardBar>();
                        // dstResidence.PutinCard(card);

                        var dstResidence = destSlot?.GetSubstantiveInParent<CardBar>(); // 如果指定了slot, 就获得指定的residence
                        dstResidence ??= GameManager.Instance.MainMan.FindSubstaintiveInChildren<CardBar>(); // 否则, 就使用main man的 card bar
                        var pl = dstResidence.GetHolder().GetComponent<ProcedureLine>(); // 获取card bar的持有者
                        pl.StartLine(new PliBuyCardForm() {
                            buyOriginal = null,
                            buyDestination = dstResidence,
                            slot = destSlot,
                            card = card,
                            free = true,
                        });
                    }
                    else if (arg.destination == typeof(CardBackpack)) {
                        // 放到卡牌背包里
                        var dstResidence = GameManager.Instance.MainMan.FindSubstaintiveInChildren<CardBackpack>();
                        dstResidence.PutinCard(card);
                    }

                    break;
                }
                case CardStore cardStore: {
                    if (arg.destination == typeof(CardStore)) {
                        // 无效
                    }
                    else if (arg.destination == typeof(CardBar)) {
                        // 购买卡牌放到卡牌栏里
                        var dstResidence = destSlot?.GetSubstantiveInParent<CardBar>(); // 如果指定了slot, 就获得指定的residence
                        dstResidence ??= GameManager.Instance.MainMan.FindSubstaintiveInChildren<CardBar>(); // 否则, 就使用main man的 card bar
                        var pl = dstResidence.GetHolder().GetComponent<ProcedureLine>(); // 获取card bar的持有者
                        pl.StartLine(new PliBuyCardForm() {
                            buyOriginal = cardStore,
                            buyDestination = dstResidence,
                            slot = destSlot,
                            card = card,
                        });
                    }
                    else if (arg.destination == typeof(CardBackpack)) {
                        // 购买卡牌放到卡牌背包里
                        var dstResidence = destSlot?.GetSubstantiveInParent<CardBackpack>(); // 如果指定了slot, 就获得指定的residence
                        dstResidence ??= GameManager.Instance.MainMan.FindSubstaintiveInChildren<CardBackpack>(); // 否则, 就使用main man的 card bar
                        var pl = dstResidence.GetHolder().GetComponent<ProcedureLine>(); // 获取card bar的持有者
                        pl.StartLine(new PliBuyCardForm() {
                            buyOriginal = cardStore,
                            buyDestination = dstResidence,
                            slot = destSlot,
                            card = card,
                        });
                    }

                    break;
                }

                case CardBar cardBar: {
                    if (arg.destination == typeof(CardStore)) {
                        // 卖掉卡牌
                        var dstResidence = destSlot?.GetSubstantiveInParent<CardStore>(); // 如果指定了slot, 就获得指定的residence
                        dstResidence ??= CardStore.Instance;
                        var pl = cardBar.GetHolder().GetComponent<ProcedureLine>();
                        pl.StartLine(new PliSellCardForm() {
                            sellOriginal = cardBar,
                            sellDestination = dstResidence,
                            card = card,
                        });
                    }
                    else if (arg.destination == typeof(CardBar)) {
                        // 调换卡牌位置
                        var slot1 = card.StaySlot;
                        var slot2 = destSlot;
                        var card1 = card;
                        var card2 = destSlot.StayCard;
                        slot1.TakeoutCard();
                        slot2.TakeoutCard();
                        if (card2 != null)
                            slot1.PutinCard(card2);
                        slot2.PutinCard(card1);
                    }
                    else if (arg.destination == typeof(CardBackpack)) {
                        // 把卡牌从卡牌栏拿到卡牌背包中
                        var dstResidence = cardBar.GetHolder().FindSubstaintiveInChildren<CardBackpack>();
                        dstResidence.PutinCard(card);
                    }

                    break;
                }

                case CardBackpack cardBackpack: {
                    if (arg.destination == typeof(CardStore)) {
                        // 卖掉卡牌
                        var dstResidence = destSlot?.GetSubstantiveInParent<CardStore>(); // 如果指定了slot, 就获得指定的residence
                        dstResidence ??= CardStore.Instance;
                        var pl = cardBackpack.GetHolder().GetComponent<ProcedureLine>();
                        pl.StartLine(new PliSellCardForm() {
                            sellOriginal = cardBackpack,
                            sellDestination = dstResidence,
                            card = card,
                        });
                    }
                    else if (arg.destination == typeof(CardBar)) {
                        // 把卡牌从卡牌背包拿到卡牌栏
                        var dstResidence = cardBackpack.GetHolder().FindSubstaintiveInChildren<CardBar>();
                        dstResidence.PutinCard(card);
                    }
                    else if (arg.destination == typeof(CardBackpack)) {
                        // 调换卡牌位置
                        var slot1 = card.StaySlot;
                        var slot2 = destSlot;
                        var card1 = card;
                        var card2 = destSlot.StayCard;
                        slot1.TakeoutCard();
                        slot2.TakeoutCard();
                        if (card2 != null)
                            slot1.PutinCard(card2);
                        slot2.PutinCard(card1);
                    }

                    break;
                }
            }
        }
    }
}