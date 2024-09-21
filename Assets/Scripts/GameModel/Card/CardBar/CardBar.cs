// using System;
// using System.Collections.Generic;
// using Hsenl.EventType;
//
// namespace Hsenl {
//     [Serializable]
//     public class CardBar : CardResidence<CardBarSlot> {
//         protected override void OnAwake() {
//             Object.DestroyChildren(this.Entity);
//
//             var orig = (int)ControlCode.Ability1;
//             for (int i = 0; i < 6; i++) {
//                 var head = Entity.Create("CardBarHeadSlot", this.Entity).AddComponent<CardBarHeadSlot>();
//                 head.confineCardType.Add(CardType.Ability);
//                 head.order = i;
//                 head.ControlCode = (ControlCode)(orig + i);
//                 for (int j = 0; j < 6; j++) {
//                     var assist = Entity.Create("CardBarAssistSlot", head.Entity).AddComponent<CardBarAssistSlot>();
//                     assist.confineCardType.Add(CardType.AbilityAssist);
//                 }
//             }
//
//             this.OnChanged();
//         }
//
//         public Card[] GetHeadCards() {
//             using var list = ListComponent<Card>.Rent();
//             foreach (var child in this.Entity.ForeachChildren()) {
//                 var card = child.GetComponent<CardBarHeadSlot>()?.StayCard;
//                 if (card == null) continue;
//                 list.Add(card);
//             }
//
//             return list.ToArray();
//         }
//
//         public override void OnPutinCard(CardBarSlot slot, Card card) {
//             switch (slot) {
//                 case CardBarHeadSlot headSlot:
//                     CardManager.Instance.EquipHeadCard(this, headSlot, card);
//                     break;
//                 case CardBarAssistSlot assistSlot:
//                     CardManager.Instance.EquipAssistCard(assistSlot, card);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//
//             base.OnPutinCard(slot, card);
//         }
//
//         public override void OnTakeoutCard(CardBarSlot slot, Card card) {
//             switch (slot) {
//                 case CardBarHeadSlot headSlot:
//                     CardManager.Instance.UnequipHeadCard(headSlot, card);
//                     break;
//                 case CardBarAssistSlot assistSlot:
//                     CardManager.Instance.UnequipAssistCard(assistSlot, card);
//                     break;
//                 default:
//                     throw new ArgumentOutOfRangeException();
//             }
//
//             base.OnTakeoutCard(slot, card);
//         }
//     }
// }