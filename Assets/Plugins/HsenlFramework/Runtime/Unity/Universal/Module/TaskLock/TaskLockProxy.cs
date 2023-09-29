using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class TaskLockProxy : MonoBehaviour {
        [SerializeField, HideLabel] private TaskLockManager taskLockManager;

        private void Awake() {
            if (!SingletonManager.IsDisposed<TaskLockManager>()) {
                SingletonManager.Unregister<TaskLockManager>();
            }
            
            SingletonManager.Register(ref this.taskLockManager);
        }

        private void Update() {
            this.taskLockManager.Update();
        }
    }
}