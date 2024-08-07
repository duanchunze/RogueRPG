﻿using System;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class PickableManager : SingletonComponent<PickableManager> {
        public Pickable Rent(int configId, Vector3 position, int count = 1) {
            var key = PoolKey.Create(typeof(Pickable), configId);
            var pickable = Pool.Rent<Pickable>(key) ?? PickableFactory.Create(configId, position, count);
            ((IPoolable)pickable).SetPoolKey(key);

            pickable.transform.Position = position;
            pickable.count = count;

            return pickable;
        }

        public void Return(Pickable pickable) {
            Pool.Return(pickable);
        }
    }
}