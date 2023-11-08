using Hsenl.behavior;

namespace Hsenl {
    [BehaviorNode]
    public abstract class CeInfo<T> : ActionNode<CasterEvaluate>, IConfigInfoInitializer<behavior.Info> where T : casterevaluate.CasterEvaluateInfo {
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
            
            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(T arg) { }
    }
}