using System;
using Hsenl.bolt;

namespace Hsenl {
    [Serializable]
    public class Bolt : Bodied, IPoolable {
        private int _configId;
        public BoltConfig Config => Tables.Instance.TbBoltConfig.GetById(this._configId);

        private PoolKey _poolKey;

        public PoolKey PoolKey => this._poolKey;

        public void Init(int configId) {
            this._configId = configId;
        }

        void IPoolable.SetPoolKey(PoolKey poolKey) {
            this._poolKey = poolKey;
        }
    }
}