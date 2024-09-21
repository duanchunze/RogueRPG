using System;

namespace Hsenl {
    [Serializable]
    public class ColliderManager : SingletonComponent<ColliderManager> {
        private readonly Type _colliderType = typeof(Collider);

        public CollisionEventListener Rent(string colliderName, bool active = true) {
            var key = PoolKey.Create(this._colliderType, colliderName);
            if (Pool.Rent(key, active: active) is not CollisionEventListener rent) {
                rent = ColliderListenerFactory.Create(colliderName, active);
            }

            return rent;
        }

        public T Rent<T>(string name = null, bool active = true) where T : Collider {
            var type = typeof(T);
            var key = PoolKey.Create(type);
            if (Pool.Rent(key, active: active) is not T rent) {
                var entity = Entity.Create(string.IsNullOrEmpty(name) ? type.Name : name);
                entity.Active = active;
                rent = entity.AddComponent<T>();
            }

            return rent;
        }

        public void Return(CollisionEventListener collisionEventListener) {
            var key = PoolKey.Create(this._colliderType, collisionEventListener.Name);
            Pool.Return(key, collisionEventListener);
        }

        public void Return<T>(T collider) where T : Collider {
            var key = PoolKey.Create(collider.GetType());
            Pool.Return(key, collider);
        }
    }
}