using System;
using MemoryPack;

namespace Hsenl {
    // 按顺序执行所有子节点, 直到某个子节点执行成功为止, 也就是 | 逻辑, 只要有一个为真, 则整体为真

    // 用例: 怪物AI (选择)
    // 选择行为: 攻击敌人(成功则持续攻击敌人) -> 巡逻(攻击敌人失败则进行巡逻)
    [Serializable]
    [MemoryPackable()]
    public partial class SelectorNode<TManager, TNode> : CompositeNode<TManager, TNode> 
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
                case NodeStatus.Success:
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
    [MemoryPackable()]
    public partial class SelectorNode : SelectorNode<BehaviorTree, Node<BehaviorTree>> { }
}