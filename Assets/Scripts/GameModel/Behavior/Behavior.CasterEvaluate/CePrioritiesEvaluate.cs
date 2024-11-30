using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable]
    public partial class CePrioritiesEvaluate : CeInfo<casterevaluate.PrioritiesEvaluateInfo> {
        private PriorityState _priorityState;

        protected override void OnEnable() {
            this._priorityState = this.manager.Bodied.GetComponent<PriorityState>();
        }

        protected override NodeStatus OnNodeTick() {
            if (this._priorityState == null) {
                return NodeStatus.Success;
            }

            var ret = this._priorityState.Evaluate(true);
            if (!ret) {
                this.manager.castEvaluateResult.CastEvaluateState = CastEvaluateState.PriorityStateEnterFailure;
            }

            return ret ? NodeStatus.Success : NodeStatus.Failure;
        }
    }
}