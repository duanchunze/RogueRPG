using MemoryPack;

namespace Hsenl {
    [MemoryPackable]
    public partial class TpSetData : TpInfo<timeline.SetDataInfo> {
        protected override void OnTimePointTrigger() {
            this.Blackboard.SetData(this.info.Key.ToString(), this.info.Value);
        }
    }
}