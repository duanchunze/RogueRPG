using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UISlot<TFiller> : MonoBehaviour, IUISlot, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler {
        public TFiller Filler { get; private set; }

        public virtual void FillIn(TFiller filler) {
            if (filler == null) {
                if (this.Filler != null) {
                    try {
                        this.OnFillerTakeout();
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }

                    this.Filler = default;
                }
            }
            else {
                this.Filler = filler;
                try {
                    this.OnFillerIn();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        protected virtual void OnFillerIn() { }

        protected virtual void OnFillerTakeout() { }

        protected virtual void OnButtonClick() { }

        protected virtual void OnRightButtonClick() { }

        protected virtual void OnPointerEnter(PointerEventData eventData) { }

        protected virtual void OnPointerExit(PointerEventData eventData) { }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Left) {
                this.OnButtonClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right) {
                this.OnRightButtonClick();
            }
        }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData) {
            this.OnPointerEnter(eventData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData) {
            this.OnPointerExit(eventData);
        }
    }
}