using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TpInflictionStatusToTarget : TpInfo<timeline.InflictionStatusToTargetInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var inflictior = ability.AttachedBodied;
                    foreach (var selectionTarget in ability.targets) {
                        Shortcut.InflictionStatus(inflictior, selectionTarget.Bodied, this.info.StatusAlias);
                    }
                    
                    break;
                }
            }
        }
    }
}