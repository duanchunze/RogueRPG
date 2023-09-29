using System;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    // 阶段, 所以子节点并行执行
    // 如果时间没到, 或者是无限时间, 则返回running
    // 相反, 则返回failure, 同时设为passed为true, 该阶段后续将不会再被执行
    [Serializable]
    [MemoryPackable()]
    public partial class Stage : ParallelNode<ITimeLine, ITimeNode>, IStageNode {
        [ShowInInspector, PropertyOrder(-1)]
        [MemoryPackInclude]
        public int StageType { get; set; }

        [ShowInInspector, PropertyOrder(-1)]
        [MemoryPackInclude]
        public float Duration { get; set; }

        [PropertyOrder(-1)]
        [MemoryPackIgnore]
        public bool passed;

        [MemoryPackIgnore]
        protected StageLine stageLine;

        protected override void OnNodeStart() {
            this.stageLine = this.manager as StageLine;
        }

        protected override void OnNodeReset() {
            this.passed = false;
        }

        protected override bool OnNodeEvaluate() {
            return this.stageLine != null && !this.passed;
        }

        protected override void OnNodeEnter() {
            this.manager.Time = 0;
            this.manager.TillTime = this.Duration;
            this.stageLine.CurrentStage = this.StageType;
        }

        protected override NodeStatus OnNodeTick() {
            var ret = base.OnNodeTick();
            if ((ret & NodeStatus.AbortStatus) == ret) {
                return ret;
            }

            return NodeStatus.Running;
        }

        protected override void OnNodeRunning() {
            if (this.manager.TillTime < 0) {
                return;
            }

            if (this.manager.Time >= this.manager.TillTime) {
                this.passed = true;
            }
        }
    }
}