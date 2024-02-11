using System;
using System.Collections.Generic;
using Hsenl.EventType;
using UnityEngine;

namespace Hsenl.View {
    public class UICardBackpack : UI<UICardBackpack> {
        public static UICardBackpack instance;

        public UnityEngine.Transform holder;
        public UnityEngine.Transform template;

        private void Awake() {
            instance = this;
        }

        private void OnDestroy() {
            instance = null;
        }

        protected override void OnOpen() {
            var cardBackpack = GameManager.Instance.MainMan?.FindScopeInBodied<CardBackpack>();
            if (cardBackpack == null) return;
            EventSystem.Publish(new OnCardResidenceChanged() { residence = cardBackpack });
        }
    }
}