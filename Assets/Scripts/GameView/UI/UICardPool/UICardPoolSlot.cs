using Hsenl.ability;
using Hsenl.ability_assist;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardPoolSlot : UIDragSlot<Bright.Config.BeanBase> {
        public Text text;

        protected override void OnFillerIn() {
            base.OnFillerIn();
            switch (this.Filler) {
                case AbilityConfig abilityConfig: {
                    this.text.text = abilityConfig.Alias;
                    break;
                }

                case AbilityPatchConfig abilityPatchConfig: {
                    this.text.text = abilityPatchConfig.Alias;
                    break;
                }

                case AbilityAssistConfig abilityAssistConfig: {
                    this.text.text = abilityAssistConfig.Alias;
                    break;
                }
            }
        }

        protected override void OnButtonClick() {
            switch (this.Filler) {
                case AbilityConfig abilityConfig: {
                    var abiBar = GameManager.Instance.MainMan?.FindBodiedInIndividual<AbilitesBar>();
                    if (abiBar != null) {
                        var abi = AbilityFactory.Create(abilityConfig);
                        abiBar.EquipAbility(abi);
                    }

                    break;
                }

                case AbilityPatchConfig abilityPatchConfig: {
                    var abiBar = GameManager.Instance.MainMan?.FindBodiedInIndividual<AbilitesBar>();
                    if (abiBar != null) {
                        var abi = abiBar.FindAbility(abilityPatchConfig.TargetAbility);
                        if (abi != null) {
                            var patch = AbilityPatchFactory.Create(abilityPatchConfig);
                            abi.AddPatch(patch);
                        }
                    }

                    break;
                }

                case AbilityAssistConfig abilityAssistConfig: {
                    break;
                }
            }
        }

        protected override void OnEndDrag(PointerEventData data) {
            switch (this.Filler) {
                case AbilityAssistConfig abilityAssistConfig: {
                    var slot = UnityHelper.UI.GetComponentInPoint<IUISlot>();
                    switch (slot) {
                        case UIAbilitySlot uiAbilitySlot: {
                            var abi = uiAbilitySlot.Filler;
                            if (abi != null) {
                                var assist = AbilityAssistFactory.Create(abilityAssistConfig);
                                abi.AddAssist(assist);
                            }
                            break;
                        }
                    }
                    break;
                }
            }
        }
    }
}