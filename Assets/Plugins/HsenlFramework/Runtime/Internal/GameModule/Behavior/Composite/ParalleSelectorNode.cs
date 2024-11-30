using System;
using MemoryPack;

namespace Hsenl {
    // selector node 和 sequential node都有一个共同特征, 如果有一个子节点处于Running状态, 那么其他的状态连Tick都不会被Tick.
    // 而ParalleSelectorNode就是在这个特征上修改为, 如果有一个子节点Running状态, 那么该节点前面的节点, 依然会被Tick.
    // 同时不同于单纯的ParalleNode, 该复合节点下同时最多只会有一个节点处于Running状态, 也就是说, 假如当前有一个Running的节点, 在下次Tick时, 其前面的某个节点返回了Running状态, 那么
    // 当前节点会停止Running, 并由前面的节点进入Running状态.
    [MemoryPackable]
    public partial class ParalleSelectorNode<TManager, TNode> : CompositeNode<TManager, TNode> 
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
                case NodeStatus.Success:
                    if (this.position > pos) {
                        this.children[this.position].AbortNode();
                    }

                    this.position = 0;
                    return NodeStatus.Success;

                case NodeStatus.Continue: // 跳过, 在这里等同于失败
                case NodeStatus.Failure:
                    pos++;
                    if (pos == this.children.Count) {
                        this.position = 0;
                        return NodeStatus.Failure;
                    }

                    goto CONTINUE;

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
                    // 正是由于这里不直接返回Break, 而是返回Continue, 所以它阻断了Break继续向上蔓延, 把Break的影响限制在了这条复合节点内部, 且由于返回的是continue, 所以不会影响上层的执行
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

        private void RunningLeave() { }
    }

    [MemoryPackable]
    public partial class ParalleSelectorNode : ParalleSelectorNode<BehaviorTree, Node<BehaviorTree>> { }
}