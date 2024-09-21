﻿// using System;
// using System.Collections.Generic;
// using Hsenl.EventType;
// using TMPro;
// using UnityEngine;
// using UnityEngine.UI;
//
// namespace Hsenl.View {
//     public class UICardStore : UI<UICardStore> {
//         public UnityEngine.Transform storeHolder;
//         public UnityEngine.Transform storeSlotTemplate;
//         public UnityEngine.Transform storePoolHolder;
//         public UnityEngine.Transform storePoolSlotTemplate;
//         public Button refreshButton;
//         public Button openClosePoolButton;
//         public TextMeshProUGUI goldText;
//         public UnityEngine.GameObject storePool;
//
//         protected override void OnCreate() {
//             this.refreshButton.onClick.AddListener(() => { CardManager.Instance.RefreshStoreCards(); });
//             this.openClosePoolButton.onClick.AddListener(() => { this.storePool.SetActive(!this.storePool.activeSelf); });
//         }
//
//         protected override void OnOpen() {
//             EventStation.OnCardResidenceUpdated(CardStore.Instance);
//         }
//     }
// }