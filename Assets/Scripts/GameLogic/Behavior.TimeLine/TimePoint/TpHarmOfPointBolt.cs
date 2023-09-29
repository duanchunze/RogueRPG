using MemoryPack;

namespace Hsenl {
    // 例如奶妈的q
    [MemoryPackable()]
    public partial class TpHarmOfPointBolt : TpInfo<timeline.HarmOfPointBoltInfo> {
        protected override void OnTimePointTrigger() {
            
        }
    }
}