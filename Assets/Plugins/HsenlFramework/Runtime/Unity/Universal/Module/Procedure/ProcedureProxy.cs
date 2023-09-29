using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Hsenl {
    [DefaultExecutionOrder(10)]
    [DisallowMultipleComponent]
    public class ProcedureProxy : MonoBehaviour {
        [LabelText("所有流程状态")]
        public string[] procedureStates;

        [ValueDropdown("ValuesGetter1"), Required, LabelText("入口流程状态")]
        public string entryProcedureState;

        [Header("Debug ------"), SerializeField, HideLabel, DisableInPlayMode, DisableInEditorMode]
        private ProcedureManager _procedureManager;

#if UNITY_EDITOR
        private string[] ValuesGetter1 => this.procedureStates;

        [OnInspectorInit]
        private void FindProcedureStates() {
            this.procedureStates = AssemblyHelper.GetSubTypeNames(typeof(AProcedureState));
        }
#endif

        private void Awake() {
            if (!SingletonManager.IsDisposed<ProcedureManager>()) {
                SingletonManager.Unregister<ProcedureManager>();
            }

            SingletonManager.Register(ref this._procedureManager);
            this._procedureManager.RegisterProcedureStates();
        }

        private void OnDestroy() {
            SingletonManager.Unregister<ProcedureManager>();
        }

        private void Start() {
            var type = EventSystem.FindType(this.entryProcedureState);
            if (type == null) throw new Exception($"type not find '{this.entryProcedureState}'");
            this._procedureManager.ChangeState(type);
        }

        private void Update() {
            this._procedureManager.Update(TimeInfo.DeltaTime);
        }
    }
}