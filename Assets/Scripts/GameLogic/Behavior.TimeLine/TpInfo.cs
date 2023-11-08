using Hsenl.behavior;
using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [BehaviorNode]
    public abstract class TpInfo<T> : TimePoint, IConfigInfoInitializer<behavior.Info> where T : timeline.TimePointInfo {
        protected T info;

        public int infoInstanceId;

        protected override void OnNodeStart() {
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
            this.point = t.Point;

            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(T arg) { }
    }
}