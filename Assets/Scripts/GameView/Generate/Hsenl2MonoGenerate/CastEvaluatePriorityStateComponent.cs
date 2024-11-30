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
    public class CastEvaluatePriorityStateComponent : MonoBehaviour, IHsenlComponentReference<Hsenl.CastEvaluatePriorityState> {
        [ReadOnly]
        public int hsenlComponentInstanceId;
    
        [SerializeField]
        private Hsenl.CastEvaluatePriorityState _hsenlComponent;
    
        public Hsenl.CastEvaluatePriorityState HsenlComponent => this._hsenlComponent;
        
        private void Awake() {
            // 如果是从资源加载go对象的话, hsenlComponentInstanceId一定等于0, 而如果是Object.InstantiateWithUnity创建的话, hsenlComponentInstanceId则一定不为0
            // 可以通过这种方式判断是预制体加载的, 还是用一个已存在的go创建的entity, 区别在于前者使用go实例化, 已经实例化好组件了, 我们直接用这些组件, 而不是新创建组件
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
            this._hsenlComponent = reference as Hsenl.CastEvaluatePriorityState;
            this.hsenlComponentInstanceId = this._hsenlComponent?.InstanceId??0;
        }
    }
    
    #if UNITY_EDITOR
    [CustomEditor(typeof(CastEvaluatePriorityStateComponent))]
    public class CastEvaluatePriorityStateComponentEditor : OdinEditor {
        private CastEvaluatePriorityStateComponent _t;

        protected override void OnEnable() {
            base.OnEnable();
            this._t = (CastEvaluatePriorityStateComponent)this.target;
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