using Hsenl.timeline;
using MemoryPack;

namespace Hsenl {
    [BehaviorNode]
    public abstract class TpInfo<T> : TimePoint, IBehaviorNodeInitializer where T : timeline.TimePointInfo {
        protected T info;

        public int infoInstanceId;

        protected override void OnNodeStart() {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = behavior.Info.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.Init(inf);
                }
            }
        }

        public void Init(behavior.Info inf) {
            var t = (T)inf;
            this.info = t;
            this.infoInstanceId = t.InstanceId;
            this.checkModel = (TimePointModel)t.Model;
            this.point = t.Point;

            this.OnInit(t);
        }

        protected virtual void OnInit(T arg) { }
    }
}