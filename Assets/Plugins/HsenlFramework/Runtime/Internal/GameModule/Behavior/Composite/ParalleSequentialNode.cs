using System;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class ParalleSequentialNode<TManager, TNode> : CompositeNode<TManager, TNode> 
        where TManager : class, IBehaviorTree where TNode : class, INode<TManager> {
        public override NodeType NodeType => NodeType.Composite;

        protected override bool OnNodeEvaluate() {
            return true;
        }

        protected override NodeStatus OnNodeTick() {
            var pos = 0;
            CONTINUE:
            var child = this.children[pos];
            var status = child.TickNode();
            switch (status) {
                case NodeStatus.Continue: // 跳过, 在这里等同于成功
                case NodeStatus.Success:
                    pos++;
                    if (pos == this.children.Count) {
                        this.position = 0;
                        return NodeStatus.Success;
                    }

                    goto CONTINUE;

                case NodeStatus.Failure:
                    if (this.position > pos) {
                        this.children[this.position].AbortNode();
                    }

                    this.position = 0;
                    return NodeStatus.Failure;

                case NodeStatus.Running:
                    if (this.position > pos) {
                        this.children[this.position].AbortNode();
                    }

                    this.position = pos;
                    return NodeStatus.Running;

                case NodeStatus.Break:
                    if (this.position > pos) {
                        this.children[this.position].AbortNode();
                    }

                    this.position = 0;
                    // 正是由于这里不直接返回Break, 而是返回Continue, 所以它阻断了Break继续向上蔓延, 把Break的影响限制在了这条复合节点内部, 且由于返回的是continue, 所以不会影响上层的
                    // 执行
                    return NodeStatus.Continue;

                case NodeStatus.Return:
                    if (this.position > pos) {
                        this.children[this.position].AbortNode();
                    }

                    this.position = 0;
                    return NodeStatus.Return; // 把Return返回给上层, 让它继续向上影响

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [MemoryPackable()]
    public partial class ParalleSequentialNode : ParalleSequentialNode<BehaviorTree, Node<BehaviorTree>> { }
}