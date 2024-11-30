using System;
using UnityEngine;
using YooAsset;

namespace Hsenl {
    [Serializable]
    public class ColliderManager : SingletonComponent<ColliderManager> {
        private readonly Type _colliderType = typeof(Collider);

        public Collider Rent(string colliderName, bool active = true) {
            var key = PoolKey.Create(this._colliderType, colliderName);
            if (Pool.Rent(key, active: active) is not Collider collider) {
                collider = null;
                if (YooAssets.LoadAssetSync(colliderName)?.AssetObject is not UnityEngine.GameObject prefab)
                    return null;

                prefab.SetActive(false);
                var go = UnityEngine.Object.Instantiate(prefab);
                go.name = colliderName;
                var go_collider = go.GetComponent<UnityEngine.Collider>();

                var entity = Entity.Create(go);
                entity.Active = active;
                
                switch (go_collider) {
                    case UnityEngine.BoxCollider:
                        collider = entity.AddComponent<BoxCollider>();
                        break;
                    case UnityEngine.SphereCollider:
                        collider = entity.AddComponent<SphereCollider>();
                        break;
                    case UnityEngine.CapsuleCollider:
                        collider = entity.AddComponent<CapsuleCollider>();
                        break;
                }
            }

            return collider;
        }

        public T Rent<T>(string name = null, bool active = true) where T : Collider {
            var type = typeof(T);
            var key = PoolKey.Create(type);
            if (Pool.Rent(key, active: active) is not T rent) {
                if (string.IsNullOrEmpty(name)) {
                    name = type.Name;
                }

                GameObject go = null;
                if (type == typeof(SphereCollider)) {
                    go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                }
                else if (type == typeof(BoxCollider)) {
                    go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                }
                else if (type == typeof(CapsuleCollider)) {
                    go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                }

                Entity entity;
                if (go != null) {
                    go.name = name;
                    entity = Entity.Create(go);
                }
                else {
                    entity = Entity.Create(name);
                }

                entity.Active = active;
                rent = entity.AddComponent<T>();
            }

            return rent;
        }

        public void Return(Collider collider) {
            var key = PoolKey.Create(this._colliderType, collider.Name);
            Pool.Return(key, collider);
        }

        public void Return<T>(T collider) where T : Collider {
            var key = PoolKey.Create(collider.GetType());
            Pool.Return(key, collider);
        }
    }
}