using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpRemoveStatusFromSelf : TpInfo<timeline.RemoveStatusFromSelfInfo>{
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var inflictior = ability.GetHolder();
                    Shortcut.TerminationStatus(inflictior, inflictior, this.info.StatusAlias);
                    break;
                }
            }
        }
    }
}