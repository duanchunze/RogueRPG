using System;
using Hsenl.actor;

namespace Hsenl {
    [Bodied(BodiedStatus.Individual)]
    [Serializable]
    public class Actor : Bodied {
        public int configId;
        
        public ActorConfig Config => Tables.Instance.TbActorConfig.GetById(this.configId);
    }
}