using System.Collections.Generic;

namespace Hsenl.ability {
    public partial class AbilityConfig {
        private readonly Dictionary<StageType, AbilityStageInfo> _stageInfos = new();
        
        partial void PostResolve() {
            foreach (var stageInfo in this.Stages) {
                this._stageInfos.Add(stageInfo.StageType, stageInfo);
            }
        }

        public AbilityStageInfo GetStageInfo(int stageType) {
            this._stageInfos.TryGetValue((StageType)stageType, out var info);
            return info;
        }
    }
}