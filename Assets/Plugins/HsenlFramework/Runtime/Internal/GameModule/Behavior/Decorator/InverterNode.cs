using System;
using MemoryPack;

namespace Hsenl {
    // 给子节点取反
    [Serializable]
    [MemoryPackable()]
    public partial class InverterNode<TManager, TNode> : DecoratorNode<TManager, TNode> where TManager : IBehaviorTree where TNode : class, INode<TManager> {
        protected override bool OnNodeEvaluate() {
            return true;
        }
        
        protected override NodeStatus OnNodeTick() {
            var status = this.child.TickNode();
            switch (status) {
                case NodeStatus.Success:
                    status = NodeStatus.Failure;
                    break;
                
                case NodeStatus.Failure:
                    status = NodeStatus.Success;
                    break;
            }

            return status;
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class InverterNode : RepeaterNode<BehaviorTree, Node<BehaviorTree>> { }
}