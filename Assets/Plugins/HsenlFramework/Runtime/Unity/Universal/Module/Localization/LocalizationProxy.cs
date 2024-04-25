using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class LocalizationProxy : MonoBehaviour {
        [ShowInInspector]
        private Localization _localization;
        
        [ValueDropdown("ValuesGetter"), Required, LabelText("当前本地化值"), OnValueChanged("OnValueChanged")]
        public string currentValue;

        private string[] values = {
            "cn",
            "en",
        };
        
#if UNITY_EDITOR
        private string[] ValuesGetter => this.values;
        private void OnValueChanged() {
            Localization.Instance?.ChangeValue(this.currentValue);
        }
#endif
        
        private void Awake() {
            if (!SingletonManager.IsDisposed<Localization>()) {
                SingletonManager.Unregister<Localization>();
            }

            SingletonManager.Register(ref this._localization);
            Localization.Instance.ChangeValue(this.currentValue);
        }
    }
}