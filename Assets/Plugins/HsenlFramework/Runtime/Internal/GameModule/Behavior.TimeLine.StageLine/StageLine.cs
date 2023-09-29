using System;
using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    // 阶段线系统是由阶段线(StageLine)和阶段(Stage)两个部分组成
    // 阶段就是包裹了若干个时间点的另一种时间线, 它和时间线有相似之处, 但更多为了服务阶段线
    // 所以阶段线采用的是"选择节点", 即阶段a执行完, 轮着b执行, b执行完, 轮着c....
    // 阶段又分为两种, 条件阶段和动作阶段, 前者是顺序节点, 后者是并行节点. 条件阶段如果必须全部执行成功, 才能继续执行后面的阶段bcd..., 如果执行不成功, 会return, 以中断整个阶段线的运行,
    //   而动作阶段, 则对执行结果没有任何要求
    [Serializable]
    [MemoryPackable()]
    public partial class StageLine : BehaviorTree<SelectorNode<ITimeLine, IStageNode>>, ITimeLine {
        [ShowInInspector, ReadOnly, LabelText("当前阶段")]
        [MemoryPackIgnore]
        protected int currentStage;

        [MemoryPackIgnore]
        public StageStatus status;

        [ShowInInspector]
        [MemoryPackIgnore]
        private float _speed = 1;

        [MemoryPackIgnore]
        public Action<int, int> onStageChanged;

        [MemoryPackIgnore]
        public int CurrentStage {
            get => this.currentStage;
            set {
                if (this.currentStage == value) return;
                var prev = this.currentStage;
                this.currentStage = value;
                this.onStageChanged?.Invoke(prev, value);
                // Debug.LogError($"current stage set '{this.currentStage}'");
            }
        }

        [MemoryPackIgnore]
        INode IBehaviorTree.CurrentNode {
            get => this.currentNode;
            set => this.currentNode = value;
        }

        [ShowInInspector]
        [MemoryPackIgnore]
        public float Time { get; set; }

        [ShowInInspector]
        [MemoryPackIgnore]
        public float TillTime { get; set; }

        [MemoryPackInclude]
        public float Speed {
            get => this.BufferSpeed;
            set {
                this._speed = value;
                this.BufferSpeed = value;
            }
        }

        [ShowInInspector]
        [MemoryPackIgnore]
        public float BufferSpeed { get; set; } = 1f;

        [ShowInInspector]
        [MemoryPackInclude]
        public float BufferLerpSpeed { get; set; } = 7.21f;

        [MemoryPackIgnore]
        public float TotalTime {
            get {
                var t = 0f;
                foreach (var child in this.entryNode.ForeachChildren()) {
                    if (child is not IStageNode stage) continue;
                    if (stage.Duration < 0) return -1;
                    t += stage.Duration;
                }

                return t;
            }
        }

        [MemoryPackIgnore]
        public TimeLineModel TimeStageLineModel {
            get {
                var totalTime = this.TotalTime;
                return totalTime switch {
                    < 0 => TimeLineModel.InfiniteTime,
                    > 0 => TimeLineModel.FiniteTime,
                    _ => TimeLineModel.Transient
                };
            }
        }

        public override void Reset() {
            this.BufferSpeed = this._speed;
            this.status = StageStatus.Running;
            this.entryNode?.ResetNode();
        }

        public override void Abort() {
            base.Abort();
            this.status = StageStatus.Finish;
            this.currentStage = 0;
        }

        public StageStatus Run(float deltaTime) {
            this.BufferSpeed = math.lerp(this.BufferSpeed, this._speed, deltaTime * this.BufferLerpSpeed);
            this.DeltaTime = deltaTime * this.Speed;
            this.Tick();
            this.Time += this.DeltaTime;
            return this.status;
        }
    }
}