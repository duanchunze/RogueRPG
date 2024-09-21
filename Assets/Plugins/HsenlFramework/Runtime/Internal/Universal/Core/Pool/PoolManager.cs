using System;
using System.Collections.Generic;

namespace Hsenl {
    // 该池针对的是框架的对象
    [Serializable]
    public sealed class PoolManager : SingletonComponent<PoolManager> {
        private readonly MultiQueue<int, Object> _pool = new(); // key: composite hashcode, value: Object
        private readonly Dictionary<string, PoolHolderCollection> _holder = new(); // key: group name, value: holder

        public Object Rent(PoolKey key, Entity parent = null, bool active = true) {
            var target = this._pool.Dequeue(key.key);
            if (target == null) {
                return null;
            }

            // 如果取出时, 没指定父级, 则暂时放到自己下面
            switch (target) {
                case Hsenl.Entity e: {
                    e.SetParent(parent ?? this.EnsureHolder(key.groupName).rent);
                    e.Active = active;
                    break;
                }

                case Component component: {
                    component.Entity.SetParent(parent ?? this.EnsureHolder(key.groupName).rent);
                    component.Entity.Active = active;
                    break;
                }
            }

            return target;
        }

        public T Rent<T>(PoolKey key, Entity parent = null, bool active = true) where T : Object, IPoolable {
            return (T)this.Rent(key, parent, active);
        }

        public void Return(PoolKey key, Object obj) {
            if (key.key == 0) {
                Log.Error(("PoolKey is can not be 0!"));
                return;
            }

            if (obj.IsDisposed)
                return;

            if (this._pool.TryGetValue(key.key, out var queue)) {
                if (queue.Count > 1000) {
                    Log.Warning($"pool return exceeds the upper limit 1000 '{nameof(obj)}'");
                    return;
                }

                if (queue.Contains(obj)) {
                    return;
                }

                queue.Enqueue(obj);
            }
            else {
                this._pool.Enqueue(key.key, obj);
            }

            switch (obj) {
                case Entity e: {
                    e.Active = false;
                    e.SetParent(this.EnsureHolder(key.groupName).ret);
                    break;
                }

                case Component component: {
                    component.Entity.Active = false;
                    component.Entity.SetParent(this.EnsureHolder(key.groupName).ret);
                    break;
                }
            }
        }

        public void Return(IPoolable poolable) {
            if (poolable == null) {
                Log.Error("Pool return is null!");
                return;
            }

            if (poolable is not Object obj) {
                Log.Error(("Poolable muse be a Hsenl.Object!"));
                return;
            }

            var key = poolable.PoolKey;
            this.Return(key, obj);
        }

        private PoolHolderCollection EnsureHolder(string holderName) {
            if (this._holder.TryGetValue(holderName, out var value)) {
                return value;
            }

            value = new PoolHolderCollection {
                // ret = Entity.Create(holderName + "Holder(Return)", this.Entity),
                // rent = Entity.Create(holderName + "Holder(Rent)", this.Entity),
                ret = Entity.Create(holderName + "Holder", this.Entity),
            };
            value.rent = value.ret;
            this._holder[holderName] = value;
            return value;
        }

        protected override void OnSingleUnregister() {
            this._pool.Clear();
            this._holder.Clear();
        }
    }

    public struct PoolKey {
        public readonly string groupName;
        public readonly int key;

        public static PoolKey Create(Type type) {
            return new PoolKey(type.Name, type.GetHashCode());
        }

        public static PoolKey Create<T>(Type type, T value) {
            return new PoolKey(type.Name, HashCode.Combine(type, value));
        }

        public static PoolKey Create(string groupName) {
            return new PoolKey(groupName, groupName.GetHashCode());
        }

        public static PoolKey Create<T>(string groupName, T value) {
            return new PoolKey(groupName, HashCode.Combine(groupName, value));
        }

        public PoolKey(string groupName, int key) {
            this.groupName = groupName;
            this.key = key;
        }
    }

    public class PoolHolderCollection {
        public Entity ret;
        public Entity rent;
    }
}