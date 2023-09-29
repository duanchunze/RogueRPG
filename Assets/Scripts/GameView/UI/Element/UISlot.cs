using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public abstract class UISlot<TFiller> : MonoBehaviour {
        public Button button;

        public TFiller Filler { get; protected set; }

        protected virtual void Start() {
            this.button.onClick.AddListener(this.OnButtonClick);
            UnityEventListener.Get(this.button).onClick += this.OnPointerClick;
        }

        protected virtual void OnDestroy() {
            this.button.onClick.RemoveListener(this.OnButtonClick);
            UnityEventListener.Get(this.button).onClick -= this.OnPointerClick;
        }

        public virtual void FillIn(TFiller filler) {
            if (filler == null) {
                if (this.Filler != null) {
                    this.OnFillerTakeout();
                    this.Filler = default;
                }
            }
            else {
                this.Filler = filler;
                this.OnFillerIn();
            }
        }

        protected virtual void OnFillerIn() { }

        protected virtual void OnFillerTakeout() { }

        protected virtual void OnButtonClick() { }

        protected virtual void OnRightButtonClick() { }


        public void OnPointerClick(PointerEventData eventData) {
            if (eventData.button == PointerEventData.InputButton.Right) {
                this.OnRightButtonClick();
            }
        }
    }
}