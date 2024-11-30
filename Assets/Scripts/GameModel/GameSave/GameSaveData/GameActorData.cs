using System;

namespace Hsenl {
    [Serializable]
    public class GameActorData {
        public string actorAlias;
        public GameActorUpgradesData upgradesData; // 升级信息
    }
}