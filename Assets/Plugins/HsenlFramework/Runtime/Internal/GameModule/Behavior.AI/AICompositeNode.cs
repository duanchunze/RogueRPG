using System;
using MemoryPack;

namespace Hsenl {
    // ai节点和并行选择节点有些许相似, 但加入了状态机特性, 即所有的节点轮询Tick, 一旦有Running, 则一直运行该节点, 并停止上一个Running节点.
    // 同时, 该节点之前的节点也不会被限制Tick. 逻辑和ET框架中的AI模块是一致的.
    // 
    // 对比其他复合节点: AI复合节点更像一个条件状态机(但允许存在空状态), 所以AI复合节点只需要两个状态, Running和Failure, 前者代表进入该节点, 后者代表退出该节点
    // 对比并行节点: AI会停留在某个Runing节点, 而并行节点不会停留在某个Running节点
    // 对比与或节点: AI是以遇到Running节点为TRUE条件, 除了Running, 其他的一律Continue(特殊状态除外)
    // 对比并行与或节点: AI是以遇到Running节点为TRUE条件
    //
    // 注意: AI子节点有三种情况会被退出, 一种是自己条件从符合到不符时, 主动退出. 一种是自己前面的节点符合条件, 强行把自己挤出去. 第三种就是来自行为树的Abort()
    [MemoryPackable()]
    public partial class AICompositeNode<TManager> : CompositeNode<TManager, INode<TManager>> where TManager : class, IBehaviorTree {
        public override NodeType NodeType => NodeType.Composite;

        [MemoryPackIgnore]
        protected Node<TManager> currentNode;
        
        protected sealed override void OnAwake() { }
        protected sealed override void OnEnable() { }
        protected sealed override void OnDisable() { }
        protected sealed override void OnReset() { }
        protected sealed override void OnAbort() { }
        protected sealed override void OnDestroy() { }
        protected sealed override void OnNodeEnter() { }
        protected sealed override void OnNodeRunStart() { }
        protected sealed override void OnNodeRunning() { }
        protected sealed override void OnNodeRunEnd() { }
        protected sealed override void OnNodeExit() { }

        protected sealed override bool OnNodeEvaluate() {
            return true;
        }

        protected sealed override NodeStatus OnNodeTick() {
            var pos = 0;
            CONTINUE:
            if (pos == this.children.Count) {
                // 不出意外情况下, 会向上层返回continue状态
                return NodeStatus.Continue;
            }

            var child = (Node<TManager>)this.children[pos];
            var oldStatus = child.NowStatus;
            var status = this.OverrideTickFrontPart(child);
            switch (status) {
                case NodeStatus.Continue:
                case NodeStatus.Success:
                case NodeStatus.Failure:
                    this.OverrideTickLatterPart(child, oldStatus);
                    pos++;
                    break;

                // 并行节点是所有节点都受限制的tick. 而AI节点是, 如果有Running节点则跳出
                case NodeStatus.Running:
                    if (child != this.currentNode) {
                        this.currentNode?.AbortNode();
                        this.currentNode = child;
                        child.InternalRunStart();
                    }

                    child.InternalRunning();
                    return NodeStatus.Running;

                case NodeStatus.Break:
                    this.OverrideTickLatterPart(child, oldStatus);
                    // 立即停止当前复合节点, 并向上层返回continue, 这里不返回break, 为的是保证break的影响只在当前复合节点之内
                    return NodeStatus.Continue;

                case NodeStatus.Return:
                    this.OverrideTickLatterPart(child, oldStatus);
                    // 立即停止当前复合节点, 且向上层返回return, 让return的影响扩散至整个行为树
                    return NodeStatus.Return;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            goto CONTINUE;
        }

        private NodeStatus OverrideTickFrontPart(Node<TManager> node) {
            if (!node.InternalEvaluate()) {
                if (node.NowStatus == NodeStatus.Running) {
                    node.InternalExit();
                }

                node.NowStatus = NodeStatus.Continue;
                return NodeStatus.Continue;
            }

            if (node.NowStatus != NodeStatus.Running) {
                node.InternalEnter();
            }

            node.NowStatus = node.InternalTick();
            return node.NowStatus;
        }

        private void OverrideTickLatterPart(Node<TManager> node, NodeStatus oldStatus) {
            if (node.NowStatus == NodeStatus.Running) {
                if (oldStatus != NodeStatus.Running) {
                    node.InternalRunStart();
                }

                node.InternalRunning();
            }
            else {
                if (oldStatus == NodeStatus.Running) {
                    node.InternalRunEnd();
                }
            }

            if (node.NowStatus != NodeStatus.Running) {
                node.InternalExit();
            }
        }
    }
}