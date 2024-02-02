using System;
using Hsenl.actor;

namespace Hsenl {
    [Serializable]
    public class Actor : Bodied {
        public int configId;
        
        public ActorConfig Config => Tables.Instance.TbActorConfig.GetById(this.configId);
    }
}