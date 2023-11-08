using Hsenl.behavior;

namespace Hsenl {
    [BehaviorNode]
    public abstract class AdvInfo<TInfo, TRecord> : ActionNode<Adventure>, IConfigInfoInitializer<behavior.Info>
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
                    this.InitInfo(inf);
                }
            }
        }

        public void InitInfo(Info configInfo) {
            var t = (TInfo)configInfo;
            this.info = t;
            this.infoInstanceId = t.InstanceId;

            this.OnConfigInfoInit(t);
        }

        protected virtual void OnConfigInfoInit(TInfo arg) { }

        protected override NodeStatus OnNodeTick() {
            return NodeStatus.Running;
        }
    }
}