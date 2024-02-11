using System;
using System.Collections.Generic;
using Hsenl.EventType;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardStore : UI<UICardStore> {
        public static UICardStore instance;

        public UnityEngine.Transform storeHolder;
        public UnityEngine.Transform storeSlotTemplate;
        public UnityEngine.Transform storePoolHolder;
        public UnityEngine.Transform storePoolSlotTemplate;
        public Button refreshButton;
        public Button openClosePoolButton;
        public TextMeshProUGUI goldText;
        public UnityEngine.GameObject storePool;

        private void Awake() {
            instance = this;
        }

        private void OnDestroy() {
            instance = null;
        }

        protected override void OnCreate() {
            this.refreshButton.onClick.AddListener(() => { EventSystem.Publish(new RefreshStoreCards()); });
            this.openClosePoolButton.onClick.AddListener(() => { this.storePool.SetActive(!this.storePool.activeSelf); });
        }

        protected override void OnOpen() {
            EventSystem.Publish(new OnCardResidenceChanged { residence = CardStore.Instance });
        }
    }
}