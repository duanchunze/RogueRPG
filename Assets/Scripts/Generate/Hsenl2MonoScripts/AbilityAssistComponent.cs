//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using UnityEngine;
using Hsenl.View;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace Hsenl.Mono {
    public class AbilityAssistComponent : MonoBehaviour, IHsenlComponentReference<AbilityAssist> {
        [ReadOnly]
        public int hsenlComponentInstanceId;
    
        [SerializeField]
        private AbilityAssist _hsenlComponent;
    
        public AbilityAssist HsenlComponent => this._hsenlComponent;
        
        private void Awake() {
            // 如果是从资源加载go对象的话, hsenlComponentInstanceId一定等于0, 而如果是Object.InstantiateWithUnity创建的话, hsenlComponentInstanceId则一定不为0
            if (this.hsenlComponentInstanceId == 0 && this._hsenlComponent != null) {
                var entity = this.GetComponent<EntityReference>()?.Entity;
                if (entity == null) {
                    entity = Entity.Create(this.gameObject);
                }
                    
                entity.AddComponent(this._hsenlComponent);
            }
        }
        
        private void Start() { }
        
        void IHsenlComponentReference.SetFrameworkReference(Component reference) {
            this._hsenlComponent = (AbilityAssist)reference;
            this.hsenlComponentInstanceId = this._hsenlComponent.InstanceId;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(AbilityAssistComponent))]
    public class AbilityAssistComponentEditor : OdinEditor {
        private AbilityAssistComponent _t;

        protected override void OnEnable() {
            base.OnEnable();
            this._t = (AbilityAssistComponent)this.target;
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