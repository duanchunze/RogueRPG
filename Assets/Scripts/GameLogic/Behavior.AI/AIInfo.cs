namespace Hsenl {
    [BehaviorNode]
    public abstract class AIInfo<T> : AINode, IBehaviorNodeInitializer where T : ai.AIInfo {
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
            
            this.OnInit(t);
        }
        
        protected virtual void OnInit(T arg) { }
    }
}