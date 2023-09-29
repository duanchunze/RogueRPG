using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public abstract class UISlot<TFiller> : MonoBehaviour {
        public TFiller Filler { get; protected set; }

        protected UnityEventListener eventListener;

        protected virtual void Start() {
            this.eventListener = UnityEventListener.Get(this);
            this.eventListener.onClick += this.OnPointerClick;
        }

        protected virtual void OnDestroy() {
            if (this.eventListener)
                this.eventListener.onClick -= this.OnPointerClick;
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
            if (eventData.button == PointerEventData.InputButton.Left) {
                this.OnButtonClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right) {
                this.OnRightButtonClick();
            }
        }
    }
}