using System;
using System.Collections.Generic;

namespace Hsenl {
    // 该池针对的是框架的对象
    [Serializable]
    public class PoolManager : SingletonComponent<PoolManager> {
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

        public T Rent<T>(PoolKey key, Entity parent = null, bool active = true) where T : Object {
            var target = this._pool.Dequeue(key.key);
            if (target == null) {
                return null;
            }

            // 如果取出时, 没指定父级, 则暂时放到自己下面
            switch (target) {
                case Entity e: {
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

            return target as T;
        }

        public void Return(PoolKey poolKey, Object obj) {
            if (obj == null)
                throw new Exception("pool return is null");

            if (obj.IsDisposed)
                return;

            if (this._pool.TryGetValue(poolKey.key, out var queue)) {
                if (queue.Count > 1000) {
                    Log.Warning($"pool return exceeds the upper limit 1000 '{obj}'");
                    return;
                }

                queue.Enqueue(obj);
            }
            else {
                this._pool.Enqueue(poolKey.key, obj);
            }

            switch (obj) {
                case Entity e: {
                    e.Active = false;
                    e.SetParent(this.EnsureHolder(poolKey.groupName).ret);
                    break;
                }

                case Component component: {
                    component.Reset();
                    component.Entity.Active = false;
                    component.Entity.SetParent(this.EnsureHolder(poolKey.groupName).ret);
                    break;
                }
            }
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
        public string groupName;
        public int key;

        public static PoolKey Create(Type type) {
            return new PoolKey() {
                groupName = type.Name,
                key = type.Name.GetHashCode(),
            };
        }

        public static PoolKey Create<T>(Type type, T value) {
            return new PoolKey {
                groupName = type.Name,
                key = HashCode.Combine(type.Name, value),
            };
        }

        public static PoolKey Create(string groupName) {
            return new PoolKey {
                groupName = groupName,
                key = groupName.GetHashCode(),
            };
        }

        public static PoolKey Create<T>(string groupName, T value) {
            return new PoolKey {
                groupName = groupName,
                key = HashCode.Combine(groupName, value),
            };
        }
    }

    public class PoolHolderCollection {
        public Entity ret;
        public Entity rent;
    }
}