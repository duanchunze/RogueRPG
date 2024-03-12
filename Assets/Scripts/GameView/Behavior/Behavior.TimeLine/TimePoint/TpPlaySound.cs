using System;
using MemoryPack;

namespace Hsenl.View {
    [Serializable]
    [MemoryPackable()]
    public partial class TpPlaySound : TpInfo<timeline.PlaySoundInfo> {
        protected override void OnTimePointTrigger() {
            var sound = this.manager.Bodied.AttachedBodied.GetComponent<Sound>();
            sound?.Play(this.info.ClipName);
        }
    }
}