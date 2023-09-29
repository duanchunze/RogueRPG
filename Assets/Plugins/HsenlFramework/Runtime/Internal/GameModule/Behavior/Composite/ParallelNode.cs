using System;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    // 并行执行所有子节点, 所有的子节点都是并行的. 不同于SelectorNode和SequentialNode两大主逻辑节点, 并行节点并不专注于门逻辑, 而只是一个为了保证所有子节点无论成功失败都能被执行的复合节点,
    // 且并行复合节点只会返回特殊状态, 详情查看OnNodeTick函数
    // 注意, 如果该节点逻辑不符合要求, 可以重写OnNodeTick函数, 实现自己的逻辑

    // 用例: 怪物AI (并行)
    // 具体攻击目标时: 发射子弹 -> 播放动画 -> 播放音效
    [Serializable]
    [MemoryPackable()]
    public partial class ParallelNode<TManager, TNode> : CompositeNode<TManager, TNode> 
        where TManager : class, IBehaviorTree where TNode : class, INode<TManager> {
        public override NodeType NodeType => NodeType.Composite;

        protected override bool OnNodeEvaluate() {
            return true;
        }

        protected override NodeStatus OnNodeTick() {
            var pos = 0;
            CONTINUE:
            if (pos == this.children.Count) {
                // 不出意外情况下, 会向上层返回continue状态
                return NodeStatus.Continue;
            }

            var child = this.children[pos];
            var status = child.TickNode();
            switch (status) {
                // 除了break和return, 其他任何状态都不会影响子节点的执行
                case NodeStatus.Continue:
                case NodeStatus.Success:
                case NodeStatus.Failure:
                case NodeStatus.Running:
                    pos++;
                    break;
                
                case NodeStatus.Break:
                    // 立即停止当前复合节点, 并向上层返回continue, 这里不返回break, 为的是保证break的影响只在当前复合节点之内
                    return NodeStatus.Continue;
                
                case NodeStatus.Return:
                    // 立即停止当前复合节点, 且向上层返回return, 让return的影响扩散至整个行为树
                    return NodeStatus.Return;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            goto CONTINUE;
        }
    }

    [Serializable]
    [MemoryPackable()]
    public partial class ParallelNode : ParallelNode<BehaviorTree, Node<BehaviorTree>> { }
}