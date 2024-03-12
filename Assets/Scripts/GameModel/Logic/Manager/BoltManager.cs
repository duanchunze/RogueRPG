using System;
using Hsenl.bolt;

namespace Hsenl {
    [Serializable]
    public class BoltManager : SingletonComponent<BoltManager> {
        private readonly Type _boltType = typeof(Bolt);

        public Bolt Rent(BoltConfig config, bool active = true) {
            var key = PoolKey.Create(this._boltType, config.GetHashCode());
            var bolt = Pool.Rent<Bolt>(key, active: active);
            if (bolt == null) {
                bolt = BoltFactory.Create(config);
                ((IPoolable)bolt).SetPoolKey(key);
            }

            return bolt;
        }

        public void Return(Bolt bolt) {
            Pool.Return(bolt);
        }
    }
}