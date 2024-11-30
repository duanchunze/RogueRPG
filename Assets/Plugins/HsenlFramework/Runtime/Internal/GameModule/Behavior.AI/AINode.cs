using System;
using MemoryPack;

namespace Hsenl {
    // 套个壳子, 好让AI类的节点更规范些
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class AINode : ActionNode<AIBehaviorTree> {
        protected sealed override void OnNodeEnter() { }
        protected sealed override void OnNodeExit() { }
        
        protected sealed override NodeStatus OnNodeTick() {
            var ret = this.Check();
            if (ret) return NodeStatus.Running;
            return NodeStatus.Failure;
        }

        protected sealed override void OnNodeRunStart() {
            this.Enter();
        }
        
        protected sealed override void OnNodeRunning() {
            this.Running();
        }
        
        protected sealed override void OnNodeRunEnd() {
            this.Exit();
        }

        // 检查是否满足条件
        protected abstract bool Check();
        protected abstract void Enter();
        protected abstract void Running();
        protected abstract void Exit();
    }
}