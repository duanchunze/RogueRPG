using System;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // 重复执行, 直到子任务失败为止(inverse == false)
    [Serializable]
    [MemoryPackable()]
    public partial class RepeaterNode<TManager, TNode> : DecoratorNode<TManager, TNode> where TManager : IBehaviorTree where TNode : class, INode<TManager> {
#if UNITY_EDITOR
        [LabelText("是否采用\"直到真为止\""), PropertyOrder(-1)]
#endif
        public bool inverse;

        protected override bool OnNodeEvaluate() {
            return true;
        }

        protected override NodeStatus OnNodeTick() {
            var status = this.child.TickNode();
            switch (status) {
                case NodeStatus.Continue:
                    return NodeStatus.Continue;

                case NodeStatus.Success:
                    return this.inverse ? NodeStatus.Success : NodeStatus.Running;

                case NodeStatus.Running:
                    return NodeStatus.Running;
                
                case NodeStatus.Failure:
                    return !this.inverse ? NodeStatus.Failure : NodeStatus.Running;
                
                case NodeStatus.Return:
                    return NodeStatus.Return;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class RepeaterNode : RepeaterNode<BehaviorTree, Node<BehaviorTree>> { }
}