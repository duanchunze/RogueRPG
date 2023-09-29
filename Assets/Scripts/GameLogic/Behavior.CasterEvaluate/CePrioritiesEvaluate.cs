using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class CePrioritiesEvaluate : CeInfo<casterevaluate.PrioritiesEvaluateInfo> {
        private PriorityState _selfPriorityState;

        protected override void OnNodeOpen() {
            switch (this.manager.Substantive) {
                case Ability ability: {
                    this._selfPriorityState = ability.GetComponent<PriorityState>();
                    break;
                }
            }
        }

        protected override NodeStatus OnNodeTick() {
            if (this._selfPriorityState == null) {
                return NodeStatus.Success;
            }

            var ret = this._selfPriorityState.EvaluateState(true);
            if (!ret) {
                this.manager.status = CastEvaluateStatus.PriorityStateEnterFailure;
            }

            return ret ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}