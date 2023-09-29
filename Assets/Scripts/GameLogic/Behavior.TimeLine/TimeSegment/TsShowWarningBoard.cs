using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class TsShowWarningBoard : TsInfo<timeline.ShowWarningBoardInfo> {
        private Entity _warningBoard;

        protected override void OnNodeReset() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    if (this._warningBoard == null) {
                        this._warningBoard = WarningBoardFactory.Create(this.info.WarnName);
                        this._warningBoard.SetParent(ability.Entity);
                    }

                    break;
                }
            }
        }

        protected override void OnTimeSegmentOrigin() {
            if (this._warningBoard == null) return;
            this._warningBoard.Active = true;
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this._warningBoard == null) return;
            this._warningBoard.Active = false;
        }
    }
}