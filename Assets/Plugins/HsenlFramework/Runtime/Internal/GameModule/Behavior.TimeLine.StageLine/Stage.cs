using System;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // 阶段, 所以子节点并行执行
    // 如果时间没到, 或者是无限时间, 则返回running
    // 相反, 则返回failure, 同时设为passed为true, 该阶段后续将不会再被执行
    [Serializable]
    [MemoryPackable()]
    public partial class Stage : ParallelNode<ITimeLine, ITimeNode>, IStageNode {
#if UNITY_EDITOR
        [ShowInInspector, PropertyOrder(-1)]
#endif
        [MemoryPackInclude]
        public int StageType { get; set; }

#if UNITY_EDITOR
        [ShowInInspector, PropertyOrder(-1)]
#endif
        [MemoryPackInclude]
        public float Duration { get; set; }

#if UNITY_EDITOR
        [PropertyOrder(-1)]
#endif
        [MemoryPackIgnore]
        public bool passed;

        [MemoryPackIgnore]
        protected StageLine stageLine;
        
        protected sealed override void OnNodeRunStart() { }
        protected sealed override void OnNodeRunEnd() { }
        protected sealed override void OnNodeExit() { }

        protected sealed override void OnAwake() {
            this.stageLine = this.manager as StageLine;
        }

        protected sealed override void OnReset() {
            this.passed = false;
        }

        protected sealed override bool OnNodeEvaluate() {
            return this.stageLine != null && !this.passed;
        }

        protected sealed override void OnNodeEnter() {
            this.manager.StageTime = 0;
            this.manager.StageTillTime = this.Duration;
            this.stageLine.CurrentStage = this.StageType;
        }

        protected sealed override NodeStatus OnNodeTick() {
            var ret = base.OnNodeTick();
            if ((ret & NodeStatus.AbortStatus) == ret) {
                return ret;
            }

            return NodeStatus.Running;
        }

        protected sealed override void OnNodeRunning() {
            if (this.manager.StageTillTime < 0) {
                return;
            }

            if (this.manager.StageTime >= this.manager.StageTillTime) {
                this.passed = true;
            }
        }
    }
}