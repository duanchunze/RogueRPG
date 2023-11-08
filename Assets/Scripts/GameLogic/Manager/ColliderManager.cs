using System;

namespace Hsenl {
    [Serializable]
    public class ColliderManager : SingletonComponent<ColliderManager> {
        private readonly Type _colliderType = typeof(Collider);

        public CollisionEventListener Rent(string colliderName, bool autoActive = true) {
            var key = PoolKey.Create(this._colliderType, colliderName);
            var rent = Pool.Rent<CollisionEventListener>(key, autoActive: autoActive);
            if (rent == null) {
                rent = ColliderFactory.Create(colliderName, autoActive);
            }

            return rent;
        }

        public T Rent<T>(string name = null, bool autoActive = true) where T : Collider {
            var type = typeof(T);
            var key = PoolKey.Create(type);
            var rent = Pool.Rent<T>(key, autoActive: autoActive);
            if (rent == null) {
                var entity = Entity.Create(string.IsNullOrEmpty(name) ? type.Name : name);
                entity.Active = autoActive;
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