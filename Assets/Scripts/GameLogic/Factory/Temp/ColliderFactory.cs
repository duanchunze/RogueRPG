using Unity.VisualScripting;
using UnityEngine;

namespace Hsenl {
    public static class ColliderFactory {
        // 不规则的Collider通过资源加载获得
        public static Collider CreateIrregularCollider(string colliderName, GameColliderPurpose colliderPurpose, bool nonEvent = false, bool enabled = true) {
            var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.ColliderBundleName, colliderName);
            var entity = Entity.Create(UnityEngine.Object.Instantiate(prefab));
            var collider = entity.AddComponent<IrregularCollider>(enabled: enabled, initializeInvoke: col => { col.NonEvent = nonEvent; });
            if ((colliderPurpose & GameColliderPurpose.Body) == GameColliderPurpose.Body) {
                collider.SetLayer(Constant.BodyLayer);
                collider.SetExcludeLayers(Constant.BodyTriggerLayerExcludeMask);
            }
            
            if ((colliderPurpose & GameColliderPurpose.BodyTrigger) == GameColliderPurpose.BodyTrigger) {
                collider.SetLayer(Constant.BodyTriggerLayer);
                collider.SetExcludeLayers(Constant.BodyLayerExcludeMask);
            }
            
            if ((colliderPurpose & GameColliderPurpose.Pickable) == GameColliderPurpose.Pickable) {
                collider.SetLayer(Constant.PickableLayer);
                collider.SetExcludeLayers(Constant.PickerLayerExcludeMask);
            }

            if ((colliderPurpose & GameColliderPurpose.Picker) == GameColliderPurpose.Picker) {
                collider.SetLayer(Constant.PickerLayer);
                collider.SetExcludeLayers(Constant.PickableLayerExcludeMask);
            }

            return collider;
        }

        public static T CreateCollider<T>(Entity entity, GameColliderPurpose colliderPurpose, bool nonEvent = false, bool enabled = true) where T : Collider {
            var collider = entity.AddComponent<T>(enabled: enabled, initializeInvoke: col => { col.NonEvent = nonEvent; });
            if ((colliderPurpose & GameColliderPurpose.Body) == GameColliderPurpose.Body) {
                collider.SetLayer(Constant.BodyLayer);
                collider.SetExcludeLayers(Constant.BodyTriggerLayerExcludeMask);
            }
            
            if ((colliderPurpose & GameColliderPurpose.BodyTrigger) == GameColliderPurpose.BodyTrigger) {
                collider.SetLayer(Constant.BodyTriggerLayer);
                collider.SetExcludeLayers(Constant.BodyLayerExcludeMask);
            }
            
            if ((colliderPurpose & GameColliderPurpose.Pickable) == GameColliderPurpose.Pickable) {
                collider.SetLayer(Constant.PickableLayer);
                collider.SetExcludeLayers(Constant.PickerLayerExcludeMask);
            }

            if ((colliderPurpose & GameColliderPurpose.Picker) == GameColliderPurpose.Picker) {
                collider.SetLayer(Constant.PickerLayer);
                collider.SetExcludeLayers(Constant.PickableLayerExcludeMask);
            }

            return collider;
        }

        public static T CreateCollider<T>(string name, GameColliderPurpose colliderPurpose, bool nonEvent = false, bool enabled = true) where T : Collider {
            var entity = Entity.Create(name);
            return CreateCollider<T>(entity, colliderPurpose, nonEvent, enabled);
        }

        public static T CreateCollider<T>(GameColliderPurpose colliderPurpose, bool nonEvent = false, bool enabled = true) where T : Collider {
            return CreateCollider<T>(typeof(T).Name, colliderPurpose, nonEvent, enabled);
        }
    }
}