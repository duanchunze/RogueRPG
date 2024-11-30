using System;
using MemoryPack;

namespace Hsenl {
    // 按顺序执行每一个子节点, 直到所有子节点都成功或者有一个子节点失败为止, 也就是 & 逻辑, 只要有一个假, 则整体为假

    // 用例: 怪物AI (顺序)
    // 攻击敌人: 条件(看到敌人) -> 条件(距离够近) -> 条件(自身处于可以发动攻击状态) -> 执行(攻击目标)
    [Serializable]
    [MemoryPackable]
    public partial class SequentialNode<TManager, TNode> : CompositeNode<TManager, TNode> 
        where TManager : class, IBehaviorTree where TNode : class, INode<TManager> {
        public override NodeType NodeType => NodeType.Composite;

        protected override bool OnNodeEvaluate() {
            return true;
        }

        protected override NodeStatus OnNodeTick() {
            var pos = this.position;
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
                    this.position = 0;
                    return NodeStatus.Failure;

                case NodeStatus.Running:
                    this.position = pos;
                    return NodeStatus.Running;
                
                case NodeStatus.Break:
                    this.position = 0;
                    // 正是由于这里不直接返回Break, 而是返回Continue, 所以它阻断了Break继续向上蔓延, 把Break的影响限制在了这条复合节点内部, 且由于返回的是continue, 所以不会影响上层的
                    // 执行
                    return NodeStatus.Continue;
                
                case NodeStatus.Return:
                    this.position = 0;
                    return NodeStatus.Return; // 把Return返回给上层, 让它继续向上影响

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    [Serializable]
    [MemoryPackable]
    public partial class SequentialNode : SequentialNode<BehaviorTree, Node<BehaviorTree>> { }
}