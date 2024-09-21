using System;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using UnityEditor;
#endif

namespace Hsenl {
    [DisallowMultipleComponent]
    public class EntityReference : MonoBehaviour, IEntityReference {
        [ShowInInspector]
        private Entity _entity;

        public Entity Entity => this._entity;

        [HideInInspector]
        public int uniqueId;

        // mono 和 framework的销毁是双向的, 无论谁在销毁时都会把对方也销毁
        private void OnDestroy() {
            var entity = this.Entity;
            if (entity == null)
                return;

            this._entity = null;
            ((IGameObjectReference)entity).SetUnityReference(null);
            Object.Destroy(entity);
        }

        void IEntityReference.SetFrameworkReference(Entity reference) {
            this._entity = reference;
            if (reference != null) {
                this.uniqueId = reference.uniqueId;
            }
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(EntityReference))]
    public class EntityReferenceEditor : OdinEditor {
        private EntityReference _t;

        protected override void OnEnable() {
            base.OnEnable();
            this._t = (EntityReference)this.target;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var entity = this._t.Entity;
            if (entity == null) {
                return;
            }

            if (entity.Active != this._t.gameObject.activeSelf) {
                entity.Active = this._t.gameObject.activeSelf;
            }

            var parentEntityRef = this._t.transform.parent?.GetComponentInParent<EntityReference>(true);
            var unityTransform1 = parentEntityRef?.transform;
            var unityTransform2 = entity.Parent?.GameObject?.transform;
            if (unityTransform1 != null && unityTransform1 != unityTransform2) {
                entity.SetParent(parentEntityRef.Entity);
            }
        }
    }
#endif
}