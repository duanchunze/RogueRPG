namespace Hsenl {
    [BehaviorNode]
    public abstract class AdvInfo<TInfo, TRecord> : ActionNode<Adventure>, IBehaviorNodeInitializer
        where TInfo : adventurescheme.AdventureInfo
        where TRecord : IRecord {
        private TRecord _record;
        protected TInfo info;

        public int infoInstanceId;

        protected TRecord Record {
            get {
                if (this._record == null) {
                    this._record = (TRecord)this.manager.Record;
                }

                return this._record;
            }
        }

        protected override void OnNodeStart() {
            if (this.info == null && this.infoInstanceId != 0) {
                var inf = behavior.Info.GetInfo(this.infoInstanceId);
                if (inf != null) {
                    this.Init(inf);
                }
            }
        }

        public void Init(behavior.Info inf) {
            var t = (TInfo)inf;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnInit(t);
        }

        protected virtual void OnInit(TInfo arg) { }

        protected override NodeStatus OnNodeTick() {
            return NodeStatus.Running;
        }
    }
}