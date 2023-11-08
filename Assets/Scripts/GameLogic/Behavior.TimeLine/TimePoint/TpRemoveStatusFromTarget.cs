using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpRemoveStatusFromTarget : TpInfo<timeline.RemoveStatusFromTargetInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var inflictior = ability.Owner;
                    foreach (var selectionTarget in ability.targets) {
                        Shortcut.TerminationStatus(inflictior, selectionTarget.Bodied, this.info.StatusAlias);
                    }

                    break;
                }
            }
        }
    }
}