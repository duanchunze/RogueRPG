using Hsenl.timeline;
using MemoryPack;

namespace Hsenl.View {
    [MemoryPackable]
    public partial class TsContinuesPlayAnim : TsInfo<ContinuesPlayAnimInfo> {
        private Motion _motion;

        protected override void OnEnable() {
            var owner = this.manager.Bodied.MainBodied;
            switch (this.manager.Bodied) {
                case Ability ability:
                    this._motion = owner?.GetComponent<Motion>();
                    break;

                case Status status:
                    this._motion = owner?.GetComponent<Motion>();
                    break;
            }
        }

        protected override void OnTimeSegmentOrigin() { }

        protected override void OnTimeSegmentRunning() {
            if (this._motion == null)
                return;

            if (!this._motion.Lock) {
                this._motion.Play(this.info.Anim, this.manager.Speed, false);
            }

            this._motion.SetSpeed(this.info.Anim, this.manager.Speed);
        }

        protected override void OnTimeSegmentTerminate(bool timeout) { }
    }
}