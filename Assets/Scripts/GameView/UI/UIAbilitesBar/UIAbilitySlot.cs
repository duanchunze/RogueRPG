using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIAbilitySlot : UIDragSlot<Ability> {
        public Image icon;
        public TextMeshProUGUI text;
        public Image cooldownMask;
        public GameObject autoTriggerMask;

        protected override void OnFillerIn() {
            base.OnFillerIn();

            // 技能槽里的显示技能, 在读条的时候, 把读条进度显示在屏幕上
            var sl = this.Filler.GetComponent<StageLine>();
            if (sl != null) {
                sl.onRunning += this.OnAbilityStageLineRunning;
            }

            try {
                var viewName = LocalizationHelper.GetAbilityLocalizationName(this.Filler.Config);
                this.text.text = viewName;
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.autoTriggerMask.SetActive(Shortcut.IsAbilityAutoCast(this.Filler));
        }

        protected override void OnFillerTakeout() {
            base.OnFillerTakeout();
            var sl = this.Filler.GetComponent<StageLine>();
            if (sl != null) {
                sl.onRunning -= this.OnAbilityStageLineRunning;
            }

            this.text.text = null;
        }

        protected override void OnButtonClick() {
            if (Shortcut.IsAbilityAutoCast(this.Filler)) {
                Shortcut.CloseAbilityAutoCast(this.Filler);
            }
            else {
                Shortcut.OpenAbilityAutoCast(this.Filler);
            }

            this.autoTriggerMask.SetActive(Shortcut.IsAbilityAutoCast(this.Filler));
        }

        protected override void OnPointerEnter(PointerEventData eventData) {
            if (this.Filler == null)
                return;

            var abiInfoUI = UIManager.SingleOpen<UIAbilityInfo>(UILayer.High);
            abiInfoUI.FillIn(this.Filler);
        }

        protected override void OnPointerExit(PointerEventData eventData) {
            UIManager.SingleClose<UIAbilityInfo>();
        }

        protected override void OnEndDrag(PointerEventData data) {
            var slot = UnityHelper.UI.GetComponentInPoint<IUISlot>();
            switch (slot) {
                case UIAbilitySlot uiAbilitySlot: {
                    var uiAbiBar = this.GetComponentInParent<UIAbilitesBar>();
                    if (uiAbiBar) {
                        uiAbiBar.AbilitesBar.SwapAbilites(this.Filler, uiAbilitySlot.Filler);
                    }

                    break;
                }
            }
        }

        private void OnAbilityStageLineRunning(int currStage, float stageTime, float stageTillTime) {
            if (!this.Filler.Tags.Contains(TagType.AbilityReading))
                return; // 如果不是读条类技能, 则不处理

            if (currStage == (int)StageType.Reading) {
                var ui = UIManager.SingleOpen<UICastReading>(UILayer.High);
                var pct = stageTime / stageTillTime;
                ui.UpdateSlider(pct);
            }
            else {
                UIManager.SingleClose<UICastReading>();
            }
        }

        private void Update() {
            if (this.Filler != null) {
                if (this.Filler.IsCooldown) {
                    this.cooldownMask.fillAmount = 0;
                }
                else {
                    var pct = this.Filler.GetCooldownPct();
                    this.cooldownMask.fillAmount = 1 - pct;
                }
            }
        }
    }
}