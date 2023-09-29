using System;
using TMPro;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UISelectHeroSlot : UISlot<actor.ActorConfig> {
        public TextMeshProUGUI text;
        public Image image;
        
        public Action onButtonClickInvoke;
        
        protected override void OnButtonClick() {
            this.onButtonClickInvoke?.Invoke();
        }
    }
}