using System;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace Hsenl.View {
    public class UIDrawCardSlot : UISlot<Bodied> {
        public Image image;
        public TextMeshProUGUI text;

        [ReadOnly]
        public int SlotInstanceId { get; set; }

        public int FillerInstanceId => this.Filler?.InstanceId ?? 0;

        protected override void OnFillerIn() {
            base.OnFillerIn();
            string viewName = null;
            switch (this.Filler) {
                case Ability ability: {
                    viewName = LocalizationHelper.GetAbilityLocalizationName(ability.Config);
                    break;
                }

                case AbilityPatch abilityPatch: {
                    viewName = LocalizationHelper.GetAbilityPatchLocalizationName(abilityPatch.Config);
                    break;
                }
            }

            this.text.text = viewName;
        }

        protected override void OnFillerTakeout() {
            base.OnFillerTakeout();
            this.text.text = null;
        }

        protected override void OnButtonClick() {
            var abilityPatch = this.Filler;
            if (abilityPatch == null)
                return;

            var drawcard = abilityPatch.FindScopeInParent<DrawCard>();
            if (drawcard == null)
                return;

            drawcard.SelectCandidate(this.Filler);
        }
    }
}