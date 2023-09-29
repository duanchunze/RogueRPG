using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpInflictionStatusToSelf : TpInfo<timeline.InflictionStatusToSelfInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var inflictior = ability.GetHolder();
                    Shortcut.InflictionStatus(inflictior, inflictior, this.info.StatusAlias);
                    break;
                }
            }
        }
    }
}