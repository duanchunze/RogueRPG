using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class CasterEvaluate : BehaviorTree<INode<CasterEvaluate>> {
        public CastEvaluateStatus status;

        public CastEvaluateStatus Evaluate(float deltaTime) {
            this.DeltaTime = deltaTime;
            var ret = this.Tick();
            
            // 评估行为的规则是, 成功代表true, running代表可以成功但要等一会, 其他的都是代表失败, 只是失败的类型很多, 这里使用一个变量, 由外部赋值
            switch (ret) {
                case NodeStatus.Success: {
                    return CastEvaluateStatus.Success;
                }

                case NodeStatus.Running: {
                    return CastEvaluateStatus.Trying;
                }

                default:
                    return this.status;
            }
        }
    }
}