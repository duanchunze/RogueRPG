//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using Hsenl.View;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace Hsenl.Mono {
    public class StatusBarComponent : MonoBehaviour, IHsenlComponentReference<StatusBar> {
        [SerializeField]
        private StatusBar _hsenlComponent;
    
        public StatusBar HsenlComponent => this._hsenlComponent;
        
        private void Awake() {
            if (this._hsenlComponent != null) {
                // 如果hsenlComponent为空, 且没有Entity, 说明该组件是由mono反序列化出来的, 需要及时添加
                if (this._hsenlComponent != null && this._hsenlComponent.IsInvalid()) {
                    var entity = this.GetComponent<EntityReference>()?.Entity;
                    if (entity == null) {
                        entity = Entity.Create(this.gameObject);
                    }
                    
                    entity.AddComponent(this._hsenlComponent);
                }
            }
        }
        
        private void Start() { }
        
        void IHsenlComponentReference.SetFrameworkReference(Component reference) {
            this._hsenlComponent = (StatusBar)reference;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(StatusBarComponent))]
    public class StatusBarComponentEditor : OdinEditor {
        private StatusBarComponent _t;

        protected override void OnEnable() {
            base.OnEnable();
            this._t = (StatusBarComponent)this.target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var hsenlComponent = this._t.HsenlComponent;
            if (hsenlComponent == null) {
                return;
            }

            if (hsenlComponent.Enable != this._t.enabled) {
                hsenlComponent.Enable = this._t.enabled;
            }
        }
    }
    #endif
}