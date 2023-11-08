using System;

namespace Hsenl {
    [Serializable]
    public class BoltManager : SingletonComponent<BoltManager> {
        private readonly Type _boltType = typeof(Bolt);

        public Bolt Rent(string bundleName, string boltName, bool autoActive = true) {
            var key = PoolKey.Create(this._boltType, HashCode.Combine(bundleName, boltName));
            var bolt = Pool.Rent<Bolt>(key, autoActive: autoActive);
            if (bolt == null) {
                bolt = BoltFactory.Create(bundleName, boltName);
                bolt.poolKey = key;
            }

            return bolt;
        }

        public void Return(Bolt bolt) {
            Pool.Return(bolt.poolKey, bolt);
        }
    }
}