using System;
using System.Collections.Generic;
using MemoryPack;
using Unity.Mathematics;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // 阶段线系统是由阶段线(StageLine)和阶段(Stage)两个部分组成
    // 阶段就是包裹了若干个时间点的另一种时间线, 它和时间线有相似之处, 但更多为了服务阶段线
    // 所以阶段线采用的是"选择节点", 即阶段a执行完, 轮着b执行, b执行完, 轮着c....
    // 阶段又分为两种, 条件阶段和动作阶段, 前者是顺序节点, 后者是并行节点. 条件阶段如果必须全部执行成功, 才能继续执行后面的阶段bcd..., 如果执行不成功, 会return, 以中断整个阶段线的运行,
    //   而动作阶段, 则对执行结果没有任何要求
    [Serializable]
    [MemoryPackable()]
    public sealed partial class StageLine : BehaviorTree<SelectorNode<ITimeLine, IStageNode>>, ITimeLine {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("当前阶段")]
#endif
        [MemoryPackIgnore]
        private int _currentStage;

        [MemoryPackIgnore]
        public StageStatus Status { get; private set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        private float _speed = 1;

        [MemoryPackIgnore]
        public Action<int, int> onStageChanged;

        [MemoryPackIgnore]
        public int CurrentStage {
            get => this._currentStage;
            set {
                if (this._currentStage == value) return;
                var prev = this._currentStage;
                this._currentStage = value;
                this.onStageChanged?.Invoke(prev, value);
                // Debug.LogError($"current stage set '{this.currentStage}'");
            }
        }

        [MemoryPackIgnore]
        INode IBehaviorTree.CurrentNode {
            get => this.currentNode;
            set => this.currentNode = value;
        }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public float Time { get; set; }

#if UNITY_EDITOR
        [ShowInInspector]
#endif
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

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public float BufferSpeed { get; set; } = 1f;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackInclude]
        public float BufferLerpSpeed { get; set; } = 7.21f;

        [MemoryPackIgnore]
        public float TotalTime {
            get {
                var t = 0f;
                Foreach(this.entryNode);

                return t;

                void Foreach(INode node) {
                    foreach (var child in node.ForeachChildren()) {
                        if (t < 0)
                            return;

                        if (child is not IStageNode stage)
                            return;

                        if (stage.Duration < 0) {
                            t = -1;
                            return;
                        }

                        t += stage.Duration;

                        Foreach(child);
                    }
                }
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

        protected internal override void OnDisposed() {
            base.OnDisposed();
            this.onStageChanged = null;
        }

        protected override void OnReset() {
            this.BufferSpeed = this._speed;
            this.Status = StageStatus.Running;
            this.entryNode?.ResetNode();
        }

        public override void Abort() {
            base.Abort();
            this.Status = StageStatus.Finish;
            this._currentStage = 0;
        }

        public StageStatus Run(float deltaTime) {
            this.BufferSpeed = math.lerp(this.BufferSpeed, this._speed, deltaTime * this.BufferLerpSpeed);
            this.DeltaTime = deltaTime * this.Speed;
            this.Tick();
            this.Time += this.DeltaTime;
            return this.Status;
        }
    }
}