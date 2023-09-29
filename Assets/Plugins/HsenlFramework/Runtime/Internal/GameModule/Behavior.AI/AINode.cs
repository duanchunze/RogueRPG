using MemoryPack;

namespace Hsenl {
    // 套个壳子, 好让AI类的节点更规范些
    public abstract partial class AINode : ActionNode<BehaviorTree> {
        protected override NodeStatus OnNodeTick() {
            var ret = this.Check();
            if (ret) return NodeStatus.Running;
            return NodeStatus.Failure;
        }

        protected override void OnNodeRunStart() {
            this.Enter();
        }
        
        protected override void OnNodeRunning() {
            this.Running();
        }
        
        protected override void OnNodeRunEnd() {
            this.Exit();
        }

        // 检查是否满足条件
        protected abstract bool Check();
        protected abstract void Enter();
        protected abstract void Running();
        protected abstract void Exit();
    }
}