using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class CoroutineProxy : MonoBehaviour {
        [SerializeField, HideLabel] private CoroutineManager _coroutineManager;

        private void Awake() {
            if (!SingletonManager.IsDisposed<CoroutineManager>()) {
                SingletonManager.Unregister<CoroutineManager>();
            }

            SingletonManager.Register(ref this._coroutineManager);
        }

        private void Update() {
            this._coroutineManager.Update();
        }
    }
}