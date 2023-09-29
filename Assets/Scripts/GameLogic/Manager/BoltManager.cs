using System;

namespace Hsenl {
    [Serializable]
    public class BoltManager : SingletonComponent<BoltManager> {
        private readonly Type _boltType = typeof(Bolt);
        
        public Bolt Rent(string boltName, bool audoActive = true) {
            var key = PoolKey.Create(this._boltType, boltName);
            if (PoolManager.Instance.Rent(key, autoActive:audoActive) is not Bolt bolt) {
                bolt = BoltFactory.Create(boltName);
            }

            return bolt;
        }

        public void Return(Bolt bolt) {
            var key = PoolKey.Create(this._boltType, bolt.Name);
            PoolManager.Instance.Return(key, bolt);
        }
    }
}