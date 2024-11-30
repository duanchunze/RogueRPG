using System;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // TimeLine采用的是ParallelNode, 所以其下的每个node都会被Tick, 所以每个node都可以时刻进行自己的计时.
    [Serializable]
    [MemoryPackable]
    public partial class TimeLine : BehaviorTree<ParallelNode<ITimeLine, ActionNode<ITimeLine>>>, ITimeLine {
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        public TimePointRunModel runModel;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public int LoopCount { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public float Time { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public float TillTime { get; set; } // < 0代表始终运行

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        public float Speed { get; set; } = 1f;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public bool IsFinish => this.runModel == TimePointRunModel.Once && this.LoopCount != 0;

        public override void Start() {
            this.Time = 0;
            this.LoopCount = 0;
            this.entryNode.StartNode();
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