using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpRemoveStatusFromSelf : TpInfo<timeline.RemoveStatusFromSelfInfo>{
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var inflictior = ability.MainBodied;
                    Shortcut.TerminationStatus(inflictior, inflictior, this.info.StatusAlias);
                    break;
                }
            }
        }
    }
}