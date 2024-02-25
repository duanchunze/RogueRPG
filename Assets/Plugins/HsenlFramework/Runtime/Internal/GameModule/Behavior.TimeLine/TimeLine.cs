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
        public float Time { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public float TillTime { get; set; }

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
                return this.TillTime switch {
                    < 0 => TimeLineModel.FiniteTime,
                    > 0 => TimeLineModel.InfiniteTime,
                    _ => TimeLineModel.Transient
                };
            }
        }

        protected override void OnReset() {
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