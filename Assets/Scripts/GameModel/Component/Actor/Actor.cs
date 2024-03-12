using System;
using Hsenl.actor;

namespace Hsenl {
    [Serializable]
    public class Actor : Bodied, IPoolable {
        public int configId;

        public ActorConfig Config => Tables.Instance.TbActorConfig.GetById(this.configId);

        private PoolKey _poolKey;

        public PoolKey PoolKey => this._poolKey;

        void IPoolable.SetPoolKey(PoolKey poolKey) {
            this._poolKey = poolKey;
        }
    }
}