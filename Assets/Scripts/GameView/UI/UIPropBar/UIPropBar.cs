using UnityEngine;

namespace Hsenl.View {
    public class UIPropBar : UI<UIPropBar> {
        public RectTransform holder;
        public UIPropSlot slotTemplate;

        public PropBar PropBar { get; private set; }

        protected override void OnOpen() {
            var propbar = GameManager.Instance.MainMan?.FindBodiedInIndividual<PropBar>();
            if (propbar != null) {
                this.FillIn(propbar);
            }
        }

        public void FillIn(PropBar propBar) {
            this.PropBar = propBar;

            var props = propBar.Props;
            var length = props.Count;
            this.holder.MakeSureChildrenCount(this.slotTemplate.transform, length);
            for (int i = 0; i < length; i++) {
                var uiSlot = this.holder.GetChild(i).GetComponent<UIPropSlot>();
                var abi = i < props.Count ? props[i] : null;
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