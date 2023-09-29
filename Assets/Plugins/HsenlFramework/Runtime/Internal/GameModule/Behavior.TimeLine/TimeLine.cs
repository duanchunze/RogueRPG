using System;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class TimeLine : BehaviorTree<ParallelNode<ITimeLine, ActionNode<ITimeLine>>>, ITimeLine {
        [ShowInInspector]
        public TimePointRunModel runModel;

        [ShowInInspector]
        public int LoopCount { get; set; }

        [ShowInInspector]
        public float Time { get; set; }

        [ShowInInspector]
        public float TillTime { get; set; }
        
        [ShowInInspector]
        public float Speed { get; set; } = 1f;

        [ShowInInspector]
        public bool IsFinish => this.runModel == TimePointRunModel.Once && this.LoopCount != 0;

        public TimeLineModel TimeLineModel {
            get {
                return this.TillTime switch {
                    < 0 => TimeLineModel.FiniteTime,
                    > 0 => TimeLineModel.InfiniteTime,
                    _ => TimeLineModel.Transient
                };
            }
        }

        public override void Reset() {
            this.Time = 0;
            this.LoopCount = 0;
            this.entryNode.ResetNode();
        }

        public void Run(float deltaTime) {
            if (this.IsFinish) return;
            if (this.TillTime >= 0) {
                if (this.Time > this.TillTime) {
                    this.LoopCount++;

                    switch (this.runModel) {
                        case TimePointRunModel.Once:
                            return;
                        case TimePointRunModel.Loop:
                            this.Time = 0;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            this.DeltaTime = deltaTime * this.Speed;
            this.Tick();
            this.Time += this.DeltaTime;
        }
    }
}