using System;
using System.Collections.Generic;

namespace Hsenl {
    [Serializable]
    public class GameSaveData : IRecord {
        public List<GameActorData> unlockedActorDatas = new();
    }
}