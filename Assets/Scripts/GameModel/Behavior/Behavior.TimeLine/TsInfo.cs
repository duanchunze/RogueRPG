using Hsenl.behavior;
using Hsenl.timeline;

namespace Hsenl {
    [BehaviorNode]
    public abstract class TsInfo<T> : TimeSegment, IConfigInfoInitializer<behavior.Info> where T : timeline.TimeSegmentInfo {
        protected T info;

        public int infoInstanceId;

        protected override void OnAwake() {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = behavior.Info.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.InitInfo(inf);
                }
            }
        }

        public void InitInfo(Info configInfo) {
            var t = (T)configInfo;
            this.info = t;
            this.infoInstanceId = t.InstanceId;
            this.checkModel = (TimePointModel)t.Model;
            this.origin = t.Origin;
            this.destination = t.Dest;

            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(T arg) { }
    }
}