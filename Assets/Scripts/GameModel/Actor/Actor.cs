using System;
using Hsenl.actor;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class Actor : Bodied, IPoolable {
        [MemoryPackInclude]
        private int _configId;

        [MemoryPackIgnore]
        public ActorConfig Config => Tables.Instance.TbActorConfig.GetById(this._configId);

        private PoolKey _poolKey;

        [MemoryPackIgnore]
        public PoolKey PoolKey => this._poolKey;

        public void SetConfigId(int configId) {
            this._configId = configId;
        }

        void IPoolable.SetPoolKey(PoolKey poolKey) {
            this._poolKey = poolKey;
        }
    }
}