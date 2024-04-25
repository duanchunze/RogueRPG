using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class ShadowFunctionProxy : MonoBehaviour {
        [SerializeField, HideLabel]
        private ShadowFunctionManager _shadowFunctionManager;

        private void Awake() {
            this._shadowFunctionManager = ShadowFunctionManager.Instance;
        }
    }
}