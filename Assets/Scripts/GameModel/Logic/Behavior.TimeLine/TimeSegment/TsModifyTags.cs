using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class TsModifyTags : TsInfo<timeline.ModifyTagsInfo> {
        public int tag;
        private Entity _target;

        protected override void OnTimeSegmentOrigin() {
            // this.manager.Substantive.Tags.Add(this.tag);
        }

        protected override void OnTimeSegmentRunning() { }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            // this.manager.Substantive.Tags.Remove(this.tag);
        }
    }
}