﻿// using Hsenl.EventType;
// using UnityEngine;
//
// namespace Hsenl.View {
//     [ShadowFunction(typeof(EventStation))]
//     public partial class ShadowEventOnCardResidenceChanged {
//         [ShadowFunction]
//         private void OnCardResidenceUpdated(Hsenl.ICardResidence residence) {
//             switch (residence) {
//                 case CardStore cardStore: {
//                     var store = UIManager.GetSingleUI<UICardStore>();
//                     if (store == null) return;
//                     store.goldText.text = GameManager.Instance.gold.ToString();
//
//                     var slots = cardStore.FindBodiedsInIndividual<CardStoreSlot>();
//                     var len = slots.Length;
//
//                     store.storeHolder.NormalizeChildren(store.storeSlotTemplate, len);
//                     for (var i = 0; i < len; i++) {
//                         var uiSlot = store.storeHolder.GetChild(i).GetComponent<UICardStoreSlot>();
//                         var slot = slots[i];
//                         uiSlot.SlotInstanceId = slot.InstanceId;
//                         uiSlot.FillIn(slot.StayCard);
//
//                         uiSlot.image.color = slot.freeze ? Color.blue : Color.white;
//                     }
//
//                     var cards = cardStore.CardPoolHoder.GetComponentsInChildren<Card>(true);
//                     len = cards.Length;
//
//                     store.storePoolHolder.NormalizeChildren(store.storePoolSlotTemplate, len);
//                     for (var i = 0; i < len; i++) {
//                         var uiSlot = store.storePoolHolder.GetChild(i).GetComponent<UICardStorePoolSlot>();
//                         var card = cards[i];
//                         uiSlot.SlotInstanceId = -1;
//                         uiSlot.FillIn(card);
//                     }
//
//                     break;
//                 }
//
//                 case CardBar cardBar: {
//                     if (cardBar.MainBodied != GameManager.Instance.MainMan) return;
//
//                     var bar = UIManager.GetSingleUI<UICardBar>();
//                     if (bar == null) return;
//
//                     var headSlots = cardBar.FindBodiedsInIndividual<CardBarHeadSlot>();
//                     bar.holder.NormalizeChildren(bar.lineTemplate.transform, headSlots.Length);
//                     bar.cardsMapToSlots.Clear();
//                     bar.abilitiesMapToSlots.Clear();
//                     bar.abilityAssistMapToSlots.Clear();
//                     for (int i = 0; i < headSlots.Length; i++) {
//                         var line = bar.holder.GetChild(i).GetComponent<UICardBarLine>();
//                         var headSlot = headSlots[i];
//                         line.headSlot.SlotInstanceId = headSlot.InstanceId;
//                         line.headSlot.FillIn(headSlot.StayCard);
//                         if (headSlot.StayCard != null) {
//                             bar.cardsMapToSlots[headSlot.StayCard] = line.headSlot;
//                             bar.abilitiesMapToSlots[(Ability)headSlot.StayCard.Source] = line.headSlot;
//                         }
//
//                         var assistSlots = headSlot.FindBodiedsInIndividual<CardBarAssistSlot>();
//                         line.assistSlotHoder.NormalizeChildren(line.assistSlotTemplate.transform, assistSlots.Length);
//                         for (int j = 0; j < assistSlots.Length; j++) {
//                             var uiAssistSlot = line.assistSlotHoder.GetChild(j).GetComponent<UICardBarAssistSlot>();
//                             var assistSlot = assistSlots[j];
//                             uiAssistSlot.SlotInstanceId = assistSlot.InstanceId;
//                             uiAssistSlot.FillIn(assistSlot.StayCard);
//
//                             if (assistSlot.StayCard != null) {
//                                 bar.cardsMapToSlots[assistSlot.StayCard] = uiAssistSlot;
//                                 bar.abilityAssistMapToSlots[(AbilityAssist)assistSlot.StayCard.Source] = uiAssistSlot;
//                             }
//                         }
//                     }
//
//                     break;
//                 }
//
//                 case CardBackpack cardBackpack: {
//                     if (cardBackpack.MainBodied != GameManager.Instance.MainMan) return;
//
//                     var backpack = UIManager.GetSingleUI<UICardBackpack>();
//                     if (backpack == null) return;
//                     var slots = cardBackpack.FindBodiedsInIndividual<CardBackpackSlot>();
//                     var len = slots.Length;
//
//                     backpack.holder.NormalizeChildren(backpack.template, len);
//
//                     for (var i = 0; i < len; i++) {
//                         var uiSlot = backpack.holder.GetChild(i).GetComponent<UICardBackpackSlot>();
//                         var slot = slots[i];
//                         uiSlot.SlotInstanceId = slot.InstanceId;
//                         uiSlot.FillIn(slot.StayCard);
//                     }
//
//                     break;
//                 }
//                 
//                 // case car
//             }
//         }
//     }
// }