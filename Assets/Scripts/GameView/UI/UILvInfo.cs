using TMPro;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UILvInfo : UI<UILvInfo> {
        public TextMeshProUGUI lvText;
        public Slider expSlider;

        protected override void OnOpen() {
            var numerator = GameManager.Instance.MainMan?.GetComponent<Numerator>();
            if (numerator != null) {
                var pct = Shortcut.GetExpPct(numerator);
                this.UpdateSlider(pct);

                var lv = Shortcut.GetLv(numerator);
                this.UpdateLv(lv);
            }
        }

        public void UpdateSlider(float value) {
            this.expSlider.value = value;
        }

        public void UpdateLv(int value) {
            this.lvText.text = value.ToString();
        }
    }
}