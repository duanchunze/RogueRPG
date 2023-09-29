namespace Hsenl {
    public abstract class QstInfo<TInfo, TRecord> : ActionNode<Quest> {
        private TRecord _record;

        protected TRecord Record {
            get {
                if (this._record == null) {
                    this._record = (TRecord)this.manager.Record;
                }

                return this._record;
            }
        }

        protected override NodeStatus OnNodeTick() {
            return NodeStatus.Running;
        }
    }
}