using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TsStatusContinuousHarm : TsStatusHarm<TsStatusContinuousHarmInfo> {
        private float _timer;

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            this._timer += TimeInfo.DeltaTime;
            if (this._timer > this.info.InternalTime) {
                this._timer = 0;

                switch (this.manager.Bodied) {
                    case Status status: {
                        var hurtable = status.MainBodied.GetComponent<Hurtable>();
                        this.Harm(hurtable, this.info.HarmFormula);
                        break;
                    }
                }
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}