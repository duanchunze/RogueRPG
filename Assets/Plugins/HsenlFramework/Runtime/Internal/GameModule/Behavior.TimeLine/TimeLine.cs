using System;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    public class TimeLine : BehaviorTree<ParallelNode<ITimeLine, ActionNode<ITimeLine>>>, ITimeLine {
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public TimePointRunModel runModel;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public int LoopCount { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public float StageTime { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public float StageTillTime { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public float Speed { get; set; } = 1f;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public bool IsFinish => this.runModel == TimePointRunModel.Once && this.LoopCount != 0;

        public TimeLineModel TimeLineModel {
            get {
                return this.StageTillTime switch {
                    < 0 => TimeLineModel.FiniteTime,
                    > 0 => TimeLineModel.InfiniteTime,
                    _ => TimeLineModel.Transient
                };
            }
        }

        public override void Reset() {
            this.StageTime = 0;
            this.LoopCount = 0;
            this.entryNode.ResetNode();
        }

        public void Run(float deltaTime) {
            if (this.IsFinish) return;
            if (this.StageTillTime >= 0) {
                if (this.StageTime > this.StageTillTime) {
                    this.LoopCount++;

                    switch (this.runModel) {
                        case TimePointRunModel.Once:
                            return;
                        case TimePointRunModel.Loop:
                            this.StageTime = 0;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            this.DeltaTime = deltaTime * this.Speed;
            this.Tick();
            this.StageTime += this.DeltaTime;
        }
    }
}