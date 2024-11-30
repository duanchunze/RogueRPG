using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl.View {
    public class UIDrawCard : UI<UIDrawCard> {
        public RectTransform holder;
        public UIDrawCardSlot elementTemplate;

        public Action onSelected;

        private DrawCard _drawCard;

        public void FillIn(DrawCard drawCard) {
            if (this._drawCard != null) {
                this._drawCard.OnSelectFinish -= this.OnSelectFinish;
            }

            this._drawCard = drawCard;
            this._drawCard.OnSelectFinish += this.OnSelectFinish;

            var patchs = drawCard.GetCandidates();
            if (patchs == null)
                return;

            this.holder.MakeSureChildrenCount(this.elementTemplate.transform, patchs.Length);
            for (int i = 0; i < patchs.Length; i++) {
                var slot = this.holder.GetChild(i)?.GetComponent<UIDrawCardSlot>();
                slot.FillIn(patchs[i]);
            }
        }

        private void OnDestroy() {
            this.onSelected = null;
            
            if (this._drawCard != null) {
                this._drawCard.OnSelectFinish -= this.OnSelectFinish;
                this._drawCard = null;
            }
        }

        private void OnSelectFinish() {
            this.onSelected?.Invoke();
        }
    }
}