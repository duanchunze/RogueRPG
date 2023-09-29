using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpRemoveStatusFromTarget : TpInfo<timeline.RemoveStatusFromTargetInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var inflictior = ability.ParentSubstantive;
                    foreach (var selectionTarget in ability.targets) {
                        Shortcut.TerminationStatus(inflictior, selectionTarget.Substantive, this.info.StatusAlias);
                    }

                    break;
                }
            }
        }
    }
}