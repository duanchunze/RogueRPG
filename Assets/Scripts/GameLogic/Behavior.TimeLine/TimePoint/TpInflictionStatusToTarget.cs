using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpInflictionStatusToTarget : TpInfo<timeline.InflictionStatusToTargetInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var inflictior = ability.ParentSubstantive;
                    foreach (var selectionTarget in ability.targets) {
                        Shortcut.InflictionStatus(inflictior, selectionTarget.Substantive, this.info.StatusAlias);
                    }
                    
                    break;
                }
            }
        }
    }
}