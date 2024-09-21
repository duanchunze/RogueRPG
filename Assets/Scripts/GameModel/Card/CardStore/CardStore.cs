// using System;
// using System.Collections.Generic;
// using Hsenl.EventType;
//
// namespace Hsenl {
//     [Serializable]
//     public class CardStore : CardResidence<CardStoreSlot> {
//         public static CardStore Instance;
//         public int capacity;
//
//         public Entity CardSlotHoder { get; private set; }
//         public Entity CardPoolHoder { get; private set; }
//
//         public List<Card> cardPool = new();
//
//         public int refreshCost = 0; // 刷新消耗金币
//         public int refreshFreeTime = 0; // 免费刷新次数
//
//         protected override void OnAwake() {
//             Instance = this;
//
//             this.refreshCost = Tables.Instance.TbGameSingletonConfig.CardStoreRefreshCost;
//
//             this.CardSlotHoder = Entity.Create("Card Slot Holder");
//             this.CardSlotHoder.SetParent(this.Entity);
//
//             this.CardPoolHoder = Entity.Create("Card Pool Holder");
//             this.CardPoolHoder.AddComponent<CardStorePool>();
//             this.CardPoolHoder.SetParent(this.Entity);
//
//             var cardConfig = Tables.Instance.TbCardConfig;
//             using var list = ListComponent<Card>.Rent();
//             foreach (var configId in Tables.Instance.TbCardSingletonConfig.CardPool) {
//                 var card = CardFactory.Create(configId);
//                 card.Entity.Active = false;
//                 list.Add(card);
//             }
//
//             this.AddCardsToPool(list.ToArray());
//             this.SetCapacity(this.capacity);
//         }
//
//         protected override void OnDestroy() {
//             Instance = null;
//         }
//
//         public void SetCapacity(int cap) {
//             this.capacity = cap;
//             this.CardSlotHoder.NormalizeChildren(this.capacity, () => {
//                 var entity = Entity.Create("CardStoreSlot");
//                 var slot = entity.AddComponent<CardStoreSlot>();
//                 slot.confineCardType.Add(CardType.Ability);
//                 slot.confineCardType.Add(CardType.AbilityAssist);
//                 slot.confineCardType.Add(CardType.AbilityPatch);
//                 return entity;
//             });
//
//             for (int i = 0, len = this.CardSlotHoder.ChildCount; i < len; i++) {
//                 var slot = this.CardSlotHoder.GetChild(i).GetComponent<CardStoreSlot>();
//             }
//         }
//
//         public void PutinCards(Card[] cards, bool cover = false) {
//             var index = 0;
//             foreach (var slot in this.FindBodiedsInIndividual<CardStoreSlot>()) {
//                 var card = slot.StayCard;
//                 if (card != null) {
//                     if (cover) {
//                         Destroy(card.Entity);
//                     }
//                     else {
//                         continue;
//                     }
//                 }
//
//                 if (index == cards.Length) break;
//                 this.PutinCard(cards[index++], slot);
//             }
//         }
//
//         public void AddCardToPool(Card card) {
//             card.SetParent(this.CardPoolHoder);
//         }
//
//         public void AddCardsToPool(params Card[] cards) {
//             foreach (var card in cards) {
//                 this.AddCardToPool(card);
//             }
//         }
//
//         public void OnAddCardToPool(Card card) {
//             this.cardPool.Add(card);
//         }
//
//         public void OnRemoveCardFromPool(Card card) {
//             this.cardPool.Remove(card);
//         }
//
//         public void RefreshCards() {
//             using var list = ListComponent<int>.Rent();
//             foreach (var card in this.cardPool) {
//                 list.Add(1);
//             }
//
//             var count = 0;
//             foreach (var storeSlot in this.FindBodiedsInIndividual<CardStoreSlot>()) {
//                 if (storeSlot.freeze)
//                     continue;
//
//                 count++;
//
//                 if (storeSlot.StayCard != null)
//                     Object.Destroy(storeSlot.StayCard.Entity);
//             }
//
//             if (count == 0)
//                 return;
//
//             var cards = RandomHelper.RandomArrayOfWeight(this.cardPool, list.ToArray(), count);
//             for (int i = 0, len = cards.Length; i < len; i++) {
//                 // cards[i] = CardFactory.Clone(cards[i]);
//                 cards[i] = Object.InstantiateWithUnity(cards[i].Entity).GetComponent<Card>();
//             }
//
//             this.PutinCards(cards);
//         }
//     }
// }