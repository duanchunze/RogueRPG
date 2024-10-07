using System;
using TMPro;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UIStatusSlot : UISlot<Status> {
        public Image icon;
        public TextMeshProUGUI text;
        public Image mask;

        protected override void OnFillerIn() {
            base.OnFillerIn();

            try {
                var viewName = this.Filler.Name;
                var stack = this.Filler.GetStack();
                if (stack != 0) {
                    this.text.text = stack.ToString();
                }
                else {
                    this.text.text = null;
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        protected override void OnFillerTakeout() {
            base.OnFillerTakeout();
            this.text.text = null;
        }

        private void Update() {
            if (this.Filler != null) { }
        }
    }
}