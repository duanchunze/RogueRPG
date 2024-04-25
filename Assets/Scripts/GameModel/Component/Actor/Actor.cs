using System;
using Hsenl.actor;

namespace Hsenl {
    [Serializable]
    public class Actor : Bodied, IPoolable {
        private int _configId;

        public ActorConfig Config => Tables.Instance.TbActorConfig.GetById(this._configId);

        private PoolKey _poolKey;

        public PoolKey PoolKey => this._poolKey;

        public void SetConfigId(int configId) {
            this._configId = configId;
        }

        void IPoolable.SetPoolKey(PoolKey poolKey) {
            this._poolKey = poolKey;
        }
    }
}