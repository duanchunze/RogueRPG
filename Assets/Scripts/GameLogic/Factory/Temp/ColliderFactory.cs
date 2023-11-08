using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Hsenl {
    public static class ColliderFactory {
        public static CollisionEventListener Create(string colliderName, bool enable = true) {
            var prefab = ResourcesHelper.GetAsset<GameObject>(Constant.ColliderBundleName, colliderName);
            prefab.SetActive(enable);
            var go = UnityEngine.Object.Instantiate(prefab);
            go.name = colliderName;
            var entity = Entity.Create(go);

            // foreach (var rigidbody in entity.GameObject.GetComponentsInChildren<UnityEngine.Rigidbody>()) {
            //     rigidbody.gameObject.GetOrCreateEntityReference().Entity.AddComponent<Rigidbody>();
            // }
            //
            // foreach (var collider in entity.GameObject.GetComponentsInChildren<UnityEngine.Collider>()) {
            //     switch (collider) {
            //         case UnityEngine.BoxCollider: {
            //             collider.gameObject.GetOrCreateEntityReference().Entity.AddComponent<BoxCollider>();
            //             break;
            //         }
            //         
            //         case UnityEngine.SphereCollider: {
            //             collider.gameObject.GetOrCreateEntityReference().Entity.AddComponent<SphereCollider>();
            //             break;
            //         }
            //         
            //         case UnityEngine.CapsuleCollider: {
            //             collider.gameObject.GetOrCreateEntityReference().Entity.AddComponent<CapsuleCollider>();
            //             break;
            //         }
            //     }
            // }

            // 这里为了简单, 就只添加了 listener, 其他的想rig collider之类的, 都不深究了
            // 所以, 碰撞体资源, 要保证, 最上级的go上, 要么有collider, 要么有rigidbody
            var listener = entity.AddComponent<CollisionEventListener>();
            return listener;
        }
    }
}