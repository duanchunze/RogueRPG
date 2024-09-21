﻿using System.Linq;
using UnityEngine;

namespace Hsenl.View {
    public class UIAbilitesBar : UI<UIAbilitesBar> {
        public RectTransform holder;
        public UIAbilitySlot slotTemplate;

        public AbilitesBar AbilitesBar { get; private set; }

        protected override void OnOpen() {
            var abibar = GameManager.Instance.MainMan?.FindBodiedInIndividual<AbilitesBar>();
            if (abibar != null) {
                this.FillIn(abibar);
            }
        }

        public void FillIn(AbilitesBar abilitesBar) {
            this.AbilitesBar = abilitesBar;

            var abilites = abilitesBar.ExplicitAbilies.Where(x => x.Tags.Contains(TagType.AbilityExplicit)).ToArray();
            this.holder.NormalizeChildren(this.slotTemplate.transform, abilitesBar.ExplicitAbilityCapacity);
            for (int i = 0; i < abilitesBar.ExplicitAbilityCapacity; i++) {
                var uiSlot = this.holder.GetChild(i).GetComponent<UIAbilitySlot>();
                var abi = i < abilites.Length ? abilites[i] : null;
                if (abi != null) {
                    uiSlot.FillIn(abi);
                }
                else {
                    uiSlot.FillIn(null);
                }
            }
        }
    }
}