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
                this.text.text = this.Filler.StackNum.ToString();
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
            if (this.Filler != null) {
                
            }
        }
    }
}