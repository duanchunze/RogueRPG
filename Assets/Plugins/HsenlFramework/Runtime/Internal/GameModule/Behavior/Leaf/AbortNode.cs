using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class AbortNode<T> : ActionNode<T> where T : IBehaviorTree {
        protected override NodeStatus OnNodeTick() {
            return NodeStatus.Return;
        }
    }
}