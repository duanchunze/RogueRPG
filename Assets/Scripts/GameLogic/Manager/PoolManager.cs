using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hsenl {
    [Serializable]
    public class PoolManager : SingletonComponent<PoolManager> {
        private readonly MultiQueue<int, Substantive> _pool = new(); // key: composite hashcode
        private readonly Dictionary<string, PoolHolderCollection> _holder = new(); // key: group name, value: holder

        public Substantive Rent(PoolKey key, Entity parent = null, bool autoActive = true) {
            var subs = this._pool.Dequeue(key.key);
            if (subs == null) {
                return null;
            }

            // 如果取出时, 没指定父级, 则暂时放到自己下面
            subs.SetParent(parent ?? this.EnsureHolder(key.typeGroup.Name).rent);
            subs.Entity.Active = autoActive;
            return subs;
        }

        public void Return(PoolKey key, Substantive substantive) {
            substantive.Entity.Active = false;
            this._pool.Enqueue(key.key, substantive);
            substantive.SetParent(this.EnsureHolder(key.typeGroup.Name).ret);
        }

        private PoolHolderCollection EnsureHolder(string holderName) {
            if (this._holder.TryGetValue(holderName, out var value)) {
                return value;
            }

            value = new PoolHolderCollection {
                ret = Entity.Create(holderName + "Holder(Return)", this.Entity),
                rent = Hsenl.Entity.Create(holderName + "Holder(Rent)", this.Entity)
            };
            this._holder[holderName] = value;
            return value;
        }
    }

    public struct PoolKey {
        public Type typeGroup;
        public int key;

        public static PoolKey Create(Type type) {
            return new PoolKey() {
                typeGroup = type,
                key = type.GetHashCode(),
            };
        }

        public static PoolKey Create<T>(Type type, T value) {
            return new PoolKey {
                typeGroup = type,
                key = HashCode.Combine(type, value),
            };
        }
    }

    public class PoolHolderCollection {
        public Entity ret;
        public Entity rent;
    }
}