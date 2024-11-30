using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIPropSlot : UIDragSlot<Prop> {
        public Image icon;
        public TextMeshProUGUI text;

        protected override void OnFillerIn() {
            base.OnFillerIn();

            try {
                var viewName = LocalizationHelper.GetPropLocalizationName(this.Filler.Config);
                this.text.text = viewName;
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnFillerTakeout() {
            base.OnFillerTakeout();
            this.text.text = null;
        }

        protected override void OnPointerEnter(PointerEventData eventData) {
            if (this.Filler == null)
                return;
        }

        protected override void OnPointerExit(PointerEventData eventData) { }

        protected override void OnEndDrag(PointerEventData data) {
            var slot = UnityHelper.UI.GetComponentInPoint<IUISlot>();
            switch (slot) {
                case UIPropSlot uiPropSlot: {
                    var uiPropBar = this.GetComponentInParent<UIPropBar>();
                    if (uiPropBar) {
                        uiPropBar.PropBar.SwapProps(this.Filler, uiPropSlot.Filler);
                    }

                    return;
                }
            }

            var tra = UnityHelper.UI.GetComponentInPoint<RectTransform>();
            if (tra != null) {
                var uicardPool = tra.GetComponentInParent<UICardPool>();
                if (uicardPool != null) {
                    Shortcut.SellCard(this.Filler);
                }
            }
        }
    }
}