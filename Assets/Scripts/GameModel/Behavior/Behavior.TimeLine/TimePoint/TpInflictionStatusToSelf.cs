using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpInflictionStatusToSelf : TpInfo<timeline.InflictionStatusToSelfInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Bodied) {
                case Ability ability: {
                    var inflictior = ability.MainBodied;
                    Shortcut.InflictionStatus(inflictior, inflictior, this.info.StatusAlias);
                    break;
                }
            }
        }
    }
}