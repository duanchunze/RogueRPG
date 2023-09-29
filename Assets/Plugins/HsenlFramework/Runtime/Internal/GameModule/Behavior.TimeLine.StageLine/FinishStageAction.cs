using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class FinishStageAction : ActionNode<ITimeLine>, IStageNode {
        public int StageType { get; } = 0;

        [MemoryPackIgnore]
        public float Duration { get; } = 0;

        protected override NodeStatus OnNodeTick() {
            this.manager.Abort();
            return NodeStatus.Return;
        }
    }
}