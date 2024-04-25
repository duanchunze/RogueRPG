using System;
using System.Collections.Generic;
using Hsenl.EventType;
using UnityEngine;

namespace Hsenl.View {
    public class UICardBar : UI<UICardBar> {
        public static UICardBar instance;

        public UnityEngine.Transform holder;
        public UICardBarLine lineTemplate;

        public readonly Dictionary<Card, UICardBarSlot> cardsMapToSlots = new();
        public readonly Dictionary<Ability, UICardBarHeadSlot> abilitiesMapToSlots = new();
        public readonly Dictionary<AbilityAssist, UICardBarAssistSlot> abilityAssistMapToSlots = new();

        private void Awake() {
            instance = this;
        }

        private void OnDestroy() {
            instance = null;
        }

        protected override void OnOpen() {
            var cardbar = GameManager.Instance.MainMan?.FindScopeInBodied<CardBar>();
            if (cardbar == null) return;
            EventStation.OnCardResidenceChanged(cardbar);
        }

        public UICardBarSlot GetSlotOfCard(Card card) {
            this.cardsMapToSlots.TryGetValue(card, out var result);
            return result;
        }
        
        public UICardBarHeadSlot GetSlotOfAbility(Ability ability) {
            this.abilitiesMapToSlots.TryGetValue(ability, out var result);
            return result;
        }
        
        public UICardBarAssistSlot GetSlotOfAbilityAssist(AbilityAssist assist) {
            this.abilityAssistMapToSlots.TryGetValue(assist, out var result);
            return result;
        }

        public void ShowAbilityAssist() {
            foreach (var barLine in this.holder.GetComponentsInChildren<UICardBarLine>(true)) {
                barLine.assistSlotHoder.gameObject.SetActive(true);
            }
        }

        public void HideAbilityAssist() {
            foreach (var barLine in this.holder.GetComponentsInChildren<UICardBarLine>(true)) {
                barLine.assistSlotHoder.gameObject.SetActive(false);
            }
        }
    }
}