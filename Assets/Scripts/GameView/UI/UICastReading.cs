using TMPro;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICastReading : UI<UICastReading> {
        public TextMeshProUGUI lvText;
        public Slider expSlider;

        public void UpdateSlider(float value) {
            this.expSlider.value = value;
        }

        public void UpdateText(int value) {
            this.lvText.text = value.ToString();
        }
    }
}