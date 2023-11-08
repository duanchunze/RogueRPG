namespace Hsenl {
    [BehaviorNode]
    public abstract class AIInfo<T> : AINode, IConfigInfoInitializer<behavior.Info> where T : ai.AIInfo {
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

        public void InitInfo(behavior.Info configInfo) {
            var t = (T)configInfo;
            this.info = t;
            this.infoInstanceId = t.InstanceId;
            
            this.OnConfigInfoInit(t);
        }
        
        protected virtual void OnConfigInfoInit(T arg) { }
    }
}