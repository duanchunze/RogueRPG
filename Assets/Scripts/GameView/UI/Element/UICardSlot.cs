using Sirenix.OdinInspector;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardSlot : UIDragSlot<Card> {
        public Image image;
        public TextMeshProUGUI text;

        [ReadOnly]
        public int SlotInstanceId { get; set; }

        public int FillerInstanceId => this.Filler?.InstanceId ?? 0;

        protected override void OnFillerIn() {
            base.OnFillerIn();
            this.text.text = this.Filler?.Config.ViewName;
        }

        protected override void OnFillerTakeout() {
            base.OnFillerTakeout();
            this.text.text = null;
        }
    }
}