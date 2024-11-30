using System;
using MemoryPack;

namespace Hsenl {
    // 固定返回状态, 无论子节点返回什么状态, 向上一律返回指定的状态
    [Serializable]
    [MemoryPackable]
    public partial class AlwayStatus<TManager, TNode> : DecoratorNode<TManager, TNode> where TManager : IBehaviorTree where TNode : class, INode<TManager> {
        public NodeStatus status;

        public AlwayStatus(NodeStatus status) {
            this.status = status;
        }

        protected override bool OnNodeEvaluate() {
            return true;
        }

        protected override NodeStatus OnNodeTick() {
            this.child.TickNode();
            return this.status;
        }
    }

    [Serializable]
    [MemoryPackable]
    public partial class AlwayStatus : AlwayStatus<BehaviorTree, Node<BehaviorTree>> {
        public AlwayStatus(NodeStatus status) : base(status) { }
    }
}