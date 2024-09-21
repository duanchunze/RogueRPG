// using System;
//
// namespace Hsenl {
//     [Serializable]
//     public class CardBarAssistSlot : CardBarSlot {
//         public override bool PutinCardEvaluate(Card card) {
//             var headSlot = this.FindScopeInParent<CardBarHeadSlot>();
//             if (headSlot == null)
//                 return false;
//
//             if (headSlot.StayCard == null)
//                 return false;
//
//             return base.PutinCardEvaluate(card);
//         }
//     }
// }