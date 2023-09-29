using System;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    public class PickableManager : SingletonComponent<PickableManager> {
        public Pickable Rent(int configId, Vector3 position, int count = 1) {
            var key = PoolKey.Create(typeof(Pickable), configId);
            if (PoolManager.Instance.Rent(key) is not Pickable pickable) {
                pickable = PickableFactory.Create(configId, position, count);
            }
            else {
                pickable.transform.Position = position;
                pickable.count = count;
            }

            return pickable;
        }

        public void Return(Pickable pickable) {
            var key = PoolKey.Create(typeof(Pickable), pickable.configId);
            PoolManager.Instance.Return(key, pickable);
        }
    }
}