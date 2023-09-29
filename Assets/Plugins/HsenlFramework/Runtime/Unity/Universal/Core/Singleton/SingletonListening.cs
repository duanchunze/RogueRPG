using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DisallowMultipleComponent]
    public class SingletonListening : MonoBehaviour {
        [ShowInInspector, LabelText("内部单例信息"), ReadOnly] private List<string> _internalSingletonInfos = new();
        [ShowInInspector, LabelText("Unity单例信息"), ReadOnly] private List<string> _unitySingletonInfos = new();

        [OnInspectorGUI]
        private void UpdateInspector() {
            this._internalSingletonInfos.Clear();
            if (SingletonManager.GetSingletons().Count != 0) {
                foreach (var kv in SingletonManager.GetSingletons()) {
                    this._internalSingletonInfos.Add($"{kv.Key.Name}");
                }
            }

            this._unitySingletonInfos.Clear();
            if (UnitySingletonManager.GetSingletons().Count != 0) {
                foreach (var kv in UnitySingletonManager.GetSingletons()) {
                    this._unitySingletonInfos.Add($"{kv.Key.Name}");
                }
            }
        }
    }
}