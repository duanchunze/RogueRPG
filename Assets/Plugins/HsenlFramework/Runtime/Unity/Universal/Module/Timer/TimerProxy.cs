using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    [DefaultExecutionOrder(-18)]
    public class TimerProxy : MonoBehaviour {
        [SerializeField, HideLabel] private TimerManager _timerManager;

        private void Awake() {
            if (!SingletonManager.IsDisposed<TimerManager>()) {
                SingletonManager.Unregister<TimerManager>();
            }
            
            SingletonManager.Register(ref this._timerManager);
        }

        private void Update() {
            this._timerManager.Update();
        }
    }
}