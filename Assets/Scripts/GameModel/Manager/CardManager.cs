// using System;
//
// namespace Hsenl {
//     [Serializable]
//     public class CardManager : SingletonComponent<CardManager> {
//         /// <summary>
//         /// 转移卡牌
//         /// </summary>
//         /// <param name="cardInstanceId">卡牌的实例id</param>
//         /// <param name="residenceType">目的地类型</param>
//         /// <param name="copyNew">转移时, 是转移本体还是拷贝一个新的</param>
//         public void TransferCard(int cardInstanceId, Type residenceType, bool copyNew = false) {
//             this.TransferCard(cardInstanceId, residenceType, null, copyNew);
//         }
//
//         /// <summary>
//         /// 转移卡牌
//         /// </summary>
//         /// <param name="cardInstanceId"></param>
//         /// <param name="destSlotInstanceId"></param>
//         /// <param name="copyNew"></param>
//         public void TransferCard(int cardInstanceId, int destSlotInstanceId, bool copyNew = false) {
//             var destSlot = EventSystem.GetInstance<CardSlot>(destSlotInstanceId);
//             if (destSlot == null)
//                 return;
//
//             var residenceType = destSlot.FindScopeInParent<ICardResidence>().GetType();
//             this.TransferCard(cardInstanceId, residenceType, destSlot, copyNew);
//         }
//
//         private void TransferCard(int cardInstanceId, Type residenceType, CardSlot destSlot, bool copyNew) {
//             var cardRaw = EventSystem.GetInstance<Card>(cardInstanceId);
//             if (cardRaw == null)
//                 return;
//
//             var card = cardRaw;
//             if (copyNew) {
//                 card = Object.InstantiateWithUnity(cardRaw.Entity).GetComponent<Card>();
//             }
//
//             var cardOriginal = cardRaw.StaySlot?.FindScopeInParent<ICardResidence>();
//             switch (cardOriginal) {
//                 case null: {
//                     if (residenceType == typeof(CardStore)) {
//                         // 无效
//                     }
//                     else if (residenceType == typeof(CardBar)) {
//                         // 放到卡牌栏里
//                         var dstResidence = destSlot?.FindScopeInParent<CardBar>(); // 如果指定了slot, 就获得指定的residence
//                         dstResidence ??= GameManager.Instance.MainMan.FindBodiedInIndividual<CardBar>(); // 否则, 就使用main man的 card bar
//                         var pl = dstResidence.MainBodied.GetComponent<ProcedureLine>(); // 获取card bar的持有者
//                         pl.StartLine(new PliBuyCardForm() {
//                             buyOriginal = null,
//                             buyDestination = dstResidence,
//                             slot = destSlot,
//                             target = card,
//                             free = true,
//                         });
//                     }
//                     else if (residenceType == typeof(CardBackpack)) {
//                         // 放到卡牌背包里
//                         var dstResidence = GameManager.Instance.MainMan.FindBodiedInIndividual<CardBackpack>();
//                         dstResidence.PutinCard(card);
//                     }
//
//                     break;
//                 }
//
//                 case CardStore cardStore: {
//                     if (residenceType == typeof(CardStore)) {
//                         // 无效
//                     }
//                     else if (residenceType == typeof(CardBar)) {
//                         // 购买卡牌放到卡牌栏里
//                         var dstResidence = destSlot?.FindScopeInParent<CardBar>(); // 如果指定了slot, 就获得指定的residence
//                         dstResidence ??= GameManager.Instance.MainMan.FindBodiedInIndividual<CardBar>(); // 否则, 就使用main man的 card bar
//                         var pl = dstResidence.MainBodied.GetComponent<ProcedureLine>(); // 获取card bar的持有者
//                         pl.StartLine(new PliBuyCardForm() {
//                             buyOriginal = cardStore,
//                             buyDestination = dstResidence,
//                             slot = destSlot,
//                             target = card,
//                         });
//                     }
//                     else if (residenceType == typeof(CardBackpack)) {
//                         // 购买卡牌放到卡牌背包里
//                         var dstResidence = destSlot?.FindScopeInParent<CardBackpack>(); // 如果指定了slot, 就获得指定的residence
//                         dstResidence ??= GameManager.Instance.MainMan.FindBodiedInIndividual<CardBackpack>(); // 否则, 就使用main man的 card bar
//                         var pl = dstResidence.MainBodied.GetComponent<ProcedureLine>(); // 获取card bar的持有者
//                         pl.StartLine(new PliBuyCardForm() {
//                             buyOriginal = cardStore,
//                             buyDestination = dstResidence,
//                             slot = destSlot,
//                             target = card,
//                         });
//                     }
//
//                     break;
//                 }
//
//                 case CardBar cardBar: {
//                     if (residenceType == typeof(CardStore)) {
//                         // 卖掉卡牌
//                         var dstResidence = destSlot?.FindScopeInParent<CardStore>(); // 如果指定了slot, 就获得指定的residence
//                         dstResidence ??= CardStore.Instance;
//                         var pl = cardBar.MainBodied.GetComponent<ProcedureLine>();
//                         pl.StartLine(new PliSellCardForm() {
//                             sellOriginal = cardBar,
//                             sellDestination = dstResidence,
//                             card = card,
//                         });
//                     }
//                     else if (residenceType == typeof(CardBar)) {
//                         // 调换卡牌位置
//                         var slot1 = card.StaySlot;
//                         var slot2 = destSlot;
//                         var card1 = card;
//                         var card2 = destSlot.StayCard;
//                         slot1.TakeoutCard();
//                         slot2.TakeoutCard();
//                         if (card2 != null)
//                             slot1.PutinCard(card2);
//                         slot2.PutinCard(card1);
//                     }
//                     else if (residenceType == typeof(CardBackpack)) {
//                         // 把卡牌从卡牌栏拿到卡牌背包中
//                         var dstResidence = cardBar.MainBodied.FindBodiedInIndividual<CardBackpack>();
//                         dstResidence.PutinCard(card);
//                     }
//
//                     break;
//                 }
//
//                 case CardBackpack cardBackpack: {
//                     if (residenceType == typeof(CardStore)) {
//                         // 卖掉卡牌
//                         var dstResidence = destSlot?.FindScopeInParent<CardStore>(); // 如果指定了slot, 就获得指定的residence
//                         dstResidence ??= CardStore.Instance;
//                         var pl = cardBackpack.MainBodied.GetComponent<ProcedureLine>();
//                         pl.StartLine(new PliSellCardForm() {
//                             sellOriginal = cardBackpack,
//                             sellDestination = dstResidence,
//                             card = card,
//                         });
//                     }
//                     else if (residenceType == typeof(CardBar)) {
//                         // 把卡牌从卡牌背包拿到卡牌栏
//                         var dstResidence = cardBackpack.MainBodied.FindBodiedInIndividual<CardBar>();
//                         dstResidence.PutinCard(card);
//                     }
//                     else if (residenceType == typeof(CardBackpack)) {
//                         // 调换卡牌位置
//                         var slot1 = card.StaySlot;
//                         var slot2 = destSlot;
//                         var card1 = card;
//                         var card2 = destSlot.StayCard;
//                         slot1.TakeoutCard();
//                         slot2.TakeoutCard();
//                         if (card2 != null)
//                             slot1.PutinCard(card2);
//                         slot2.PutinCard(card1);
//                     }
//
//                     break;
//                 }
//             }
//         }
//
//         // 冻结卡牌
//         public void FreezeCard(int cardInstanceId) {
//             var card = EventSystem.GetInstance<Card>(cardInstanceId);
//             if (card == null)
//                 return;
//
//             if (card.StaySlot is not CardStoreSlot cardStoreSlot)
//                 return;
//
//             cardStoreSlot.freeze = !cardStoreSlot.freeze;
//             var cardStore = cardStoreSlot.FindScopeInParent<CardStore>();
//             cardStore.OnChanged();
//         }
//
//         // 切换技能自动触发
//         public void ChangeAbilityAutoTrigger(int cardInstanceId, int cardBarSlotInstanceId) {
//             var card = EventSystem.GetInstance<Card>(cardInstanceId);
//             if (card == null)
//                 throw new Exception($"cant find card with instance id '{cardInstanceId}'");
//
//             var slot = EventSystem.GetInstance<CardBarHeadSlot>(cardBarSlotInstanceId);
//             if (slot == null)
//                 throw new Exception($"cant find card bar head slot with instance id '{cardBarSlotInstanceId}'");
//
//             var cardBar = slot.FindScopeInParent<CardBar>();
//             if (cardBar == null)
//                 throw new Exception($"get card bar fail");
//
//             if (card.Source is Ability ability) {
//                 var controlTrigger = ability.GetComponent<ControlTrigger>();
//                 if (controlTrigger.ControlCode == (int)ControlCode.AutoTrigger) {
//                     controlTrigger.ControlCode = (int)slot.ControlCode;
//                 }
//                 else {
//                     controlTrigger.ControlCode = (int)ControlCode.AutoTrigger;
//                 }
//
//                 cardBar.OnChanged();
//             }
//         }
//
//         // 刷新商店卡牌
//         public void RefreshStoreCards() {
//             GameManager.Instance.ProcedureLine.StartLine(new PliRefreshStoreCardsForm());
//         }
//
//         // 展示选牌
//         public async HTask DrawCards(Bodied target) {
//             await GameManager.Instance.ProcedureLine.StartLineAsync(new PliDrawCardForm() {
//                 target = target,
//             });
//         }
//
//         public void EquipAssistCard(CardBarAssistSlot slot, Card card) {
//             var headCardSlot = slot.FindScopeInParent<CardBarHeadSlot>();
//             var headCard = headCardSlot.StayCard;
//             if (headCard == null)
//                 throw new Exception("head card not exist");
//
//             switch (card.Source) {
//                 case AbilityAssist abilityAssist: {
//                     abilityAssist.SetParent(headCard.Source.Entity);
//                     abilityAssist.transform.NormalTransfrom();
//                     break;
//                 }
//             }
//         }
//
//         public void UnequipAssistCard(CardBarAssistSlot slot, Card card) {
//             switch (card.Source) {
//                 case AbilityAssist abilityAssist: {
//                     abilityAssist.SetParent(slot.Entity);
//                     abilityAssist.transform.NormalTransfrom();
//                     break;
//                 }
//             }
//         }
//
//         public void EquipHeadCard(CardBar cardBar, CardBarHeadSlot slot, Card card) {
//             switch (card.Source) {
//                 case Ability ability: {
//                     var abilityBar = cardBar.MainBodied.FindBodiedInIndividual<AbilityBar>();
//                     var trigger = ability.GetComponent<ControlTrigger>();
//                     if (trigger != null) {
//                         // 如果技能本身的控制码==0, 那就用槽的控制码作为他的控制码
//                         if (trigger.ControlCode == 0) {
//                             trigger.ControlCode = (int)slot.ControlCode;
//                         }
//                     }
//
//                     abilityBar.EquipAbility(ability);
//                     break;
//                 }
//             }
//         }
//
//         public void UnequipHeadCard(CardBarHeadSlot slot, Card card) {
//             switch (card.Source) {
//                 case Ability ability: {
//                     var holder = GameManager.Instance.MainMan.FindBodiedInIndividual<AbilityBar>();
//                     holder.UnequipAbility(ability);
//                     ability.GetLinker<Card>().Reset();
//                     break;
//                 }
//             }
//         }
//     }
// }