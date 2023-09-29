using Hsenl.timeline;

namespace Hsenl {
    [BehaviorNode]
    public abstract class TsInfo<T> : TimeSegment, IBehaviorNodeInitializer where T : timeline.TimeSegmentInfo {
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
            this.origin = t.Origin;
            this.destination = t.Dest;

            this.OnInit(t);
        }

        protected virtual void OnInit(T arg) { }
    }
}