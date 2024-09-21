// using System;
// using System.Collections.Generic;
// using Hsenl.EventType;
// using UnityEngine;
//
// namespace Hsenl.View {
//     public class UICardBackpack : UI<UICardBackpack> {
//         public UnityEngine.Transform holder;
//         public UnityEngine.Transform template;
//
//         protected override void OnOpen() {
//             var cardBackpack = GameManager.Instance.MainMan?.FindBodiedInIndividual<CardBackpack>();
//             if (cardBackpack == null) return;
//             EventStation.OnCardResidenceUpdated(cardBackpack);
//         }
//     }
// }