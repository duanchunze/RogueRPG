using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-19)]
    public class TimeProxy : MonoBehaviour {
        [OnValueChanged("OnTimeScaleChanged"), PropertyRange(0f, 5f)] public float timeScale = 1;
        [SerializeField, HideLabel] private TimeInfoManager _timeInfoManager;

        private void OnTimeScaleChanged() {
            Time.timeScale = this.timeScale;
        }

        private void Awake() {
            if (!SingletonManager.IsDisposed<TimeInfoManager>()) {
                SingletonManager.Unregister<TimeInfoManager>();
            }
            
            SingletonManager.Register(ref this._timeInfoManager);
            this.StartCoroutine(nameof(this.InternalUpdate));
        }

        private IEnumerator InternalUpdate() {
            var wait = new WaitForEndOfFrame();
            while (true) {
                yield return wait;
                this._timeInfoManager.Update();
            }
        }
    }
}