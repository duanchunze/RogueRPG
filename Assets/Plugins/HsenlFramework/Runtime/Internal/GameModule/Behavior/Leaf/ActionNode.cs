using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ActionNode<T> : LeafNode<T> where T : IBehaviorTree {
        public override NodeType NodeType => NodeType.Action;
        
        protected override bool OnNodeEvaluate() {
            return true;
        }
    }

    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class ActionNode : ActionNode<BehaviorTree> { }
}