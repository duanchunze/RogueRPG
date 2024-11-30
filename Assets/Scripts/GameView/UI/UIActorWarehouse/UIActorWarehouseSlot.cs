using UnityEngine.UI;

namespace Hsenl.View {
    public class UIActorWarehouseSlot : UISlot<actor.ActorConfig> {
        public Text text;

        protected override void OnFillerIn() {
            this.text.text = this.Filler.Alias;
        }
    }
}