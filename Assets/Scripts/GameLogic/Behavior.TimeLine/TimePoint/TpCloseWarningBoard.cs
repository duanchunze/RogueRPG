using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TpCloseWarningBoard : TpInfo<timeline.CloseWarningBoardInfo> {
        protected override void OnTimePointTrigger() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var warnBoard = ability.FindChild(this.info.WarnName);
                    if (warnBoard != null) {
                        warnBoard.Active = false;
                    }

                    break;
                }
            }
        }

        protected override void OnNodeAbort() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    var warnBoard = ability.FindChild(this.info.WarnName);
                    if (warnBoard != null) {
                        warnBoard.Active = false;
                    }

                    break;
                }
            }
        }
    }
}