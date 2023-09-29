using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TsRanshao : TsHarm<RanshaoInfo> {
        private float _timer;

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            this._timer += TimeInfo.DeltaTime;
            if (this._timer > this.info.InternalTime) {
                this._timer = 0;

                switch (this.manager.Substantive) {
                    case Status status: {
                        var hurtable = status.GetHolder().GetComponent<Hurtable>();
                        this.Harm(hurtable);
                        break;
                    }
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}