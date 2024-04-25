using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class PriorityState : Unbodied, IPriorityState {
        private bool _handledFlag;

        /// 该时间缩放应该用在游戏逻辑上, 而不是用在比如击败boss的慢动作上, 那个应该用TimeInfo.TimeScale
#if UNITY_EDITOR
        [SerializeField, PropertyRange(0f, 5f), LabelText("时间缩放"), FoldoutGroup("优先级")]
#endif
        public float timeScale = 1f;

#if UNITY_EDITOR
        [SerializeField, LabelText("通道"), FoldoutGroup("优先级")]
#endif
        public int[] aisles = { 0 };

#if UNITY_EDITOR
        [SerializeField, LabelText("进入等级"), FoldoutGroup("优先级")]
#endif
        public int enterPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("阻拦等级锚点"), FoldoutGroup("优先级")]
#endif
        public int resistPriorityAnchor;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("阻拦等级"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        protected int resistPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("排挤等级"), FoldoutGroup("优先级")]
#endif
        public int exclusionPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("保持等级"), FoldoutGroup("优先级")]
#endif
        public int keepPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("禁用等级"), FoldoutGroup("优先级")]
#endif
        public int disablePriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("运行等级"), FoldoutGroup("优先级")]
#endif
        public int runPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定通过"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialPassLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定拦截"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialInterceptLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定排挤"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialExclusionLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定保持"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialKeepLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定禁用"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialDisableLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定运行"), FoldoutGroup("优先级")]
#endif
        public Bitlist specialRunLabels;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("上一帧暂停"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        protected bool pausedPrevious;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("暂停"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        protected bool paused;

#if UNITY_EDITOR
        [SerializeField, ReadOnly, LabelText("持续时间"), FoldoutGroup("优先级")]
#endif
        public float duration;

#if UNITY_EDITOR
        [SerializeField, ReadOnly, LabelText("当前时间"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        protected float time;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("时间暂停"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        public bool timeParse;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("状态管理器"), FoldoutGroup("优先级")]
#endif
        private IPrioritizer _manager;

#if UNITY_EDITOR
        [LabelText("允许重进"), FoldoutGroup("优先级")]
#endif
        public bool allowReenter;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        private IPrioritizer _targetPrioritizer;

        [MemoryPackIgnore]
        public IPrioritizer TargetPrioritizer {
            set {
                this._targetPrioritizer = value;
                if (this._targetPrioritizer == null) {
                    this.LeaveState();
                }
            }
        }
        
        protected override void OnDisable() {
            this.LeaveState();
        }

        protected internal override void OnDisposed() {
            base.OnDisposed();
            this._handledFlag = false;
            this.timeScale = 1f;
            this.aisles = null;
            this.enterPriority = 0;
            this.resistPriorityAnchor = 0;
            this.resistPriority = 0;
            this.keepPriority = 0;
            this.exclusionPriority = 0;
            this.runPriority = 0;
            this.disablePriority = 0;
            this.specialPassLabels?.Clear();
            this.specialInterceptLabels?.Clear();
            this.specialExclusionLabels?.Clear();
            this.specialKeepLabels?.Clear();
            this.specialDisableLabels?.Clear();
            this.specialRunLabels?.Clear();
            this.pausedPrevious = false;
            this.paused = false;
            this.duration = 0;
            this.time = 0;
            this.timeParse = false;
            this._manager = null;
            this.allowReenter = false;
            this.TargetPrioritizer = null;
        }

        bool IPriorityState.HandledFlag {
            get => this._handledFlag;
            set => this._handledFlag = value;
        }

        float IPriorityState.TimeScale {
            get => this.timeScale;
            set => this.timeScale = value;
        }

        string IPriorityState.Name {
            get => this.Entity.name;
            set => this.Entity.name = value;
        }

        IList<int> IPriorityState.Aisles {
            get => this.aisles;
            set => this.aisles = value.ToArray();
        }

        Bitlist IPriorityState.Labels {
            get => this.Tags;
            set => throw new InvalidOperationException();
        }

        int IPriorityState.EnterPriority {
            get => this.enterPriority;
            set => this.enterPriority = value;
        }

        int IPriorityState.ResistPriorityAnchor {
            get => this.resistPriorityAnchor;
            set => this.resistPriorityAnchor = value;
        }

        int IPriorityState.ResistPriority {
            get => this.resistPriority;
            set => this.resistPriority = value;
        }

        int IPriorityState.ExclusionPriority {
            get => this.exclusionPriority;
            set => this.exclusionPriority = value;
        }

        int IPriorityState.KeepPriority {
            get => this.keepPriority;
            set => this.keepPriority = value;
        }

        int IPriorityState.DisablePriority {
            get => this.disablePriority;
            set => this.disablePriority = value;
        }

        int IPriorityState.RunPriority {
            get => this.runPriority;
            set => this.runPriority = value;
        }

        Bitlist IPriorityState.SpecialPassLabels {
            get => this.specialPassLabels;
            set => this.specialPassLabels = value;
        }

        Bitlist IPriorityState.SpecialInterceptLabels {
            get => this.specialInterceptLabels;
            set => this.specialInterceptLabels = value;
        }

        Bitlist IPriorityState.SpecialExclusionLabels {
            get => this.specialExclusionLabels;
            set => this.specialExclusionLabels = value;
        }

        Bitlist IPriorityState.SpecialKeepLabels {
            get => this.specialKeepLabels;
            set => this.specialKeepLabels = value;
        }

        Bitlist IPriorityState.SpecialDisableLabels {
            get => this.specialDisableLabels;
            set => this.specialDisableLabels = value;
        }

        Bitlist IPriorityState.SpecialRunLabels {
            get => this.specialRunLabels;
            set => this.specialRunLabels = value;
        }

        bool IPriorityState.PausedPrevious {
            get => this.pausedPrevious;
            set => this.pausedPrevious = value;
        }

        bool IPriorityState.Paused {
            get => this.paused;
            set => this.paused = value;
        }

        float IPriorityState.Duration {
            get => this.duration;
            set => this.duration = value;
        }

        float IPriorityState.Time {
            get => this.time;
            set => this.time = value;
        }

        bool IPriorityState.TimeParse {
            get => this.timeParse;
            set => this.timeParse = value;
        }

        IPrioritizer IPriorityState.Manager {
            get => this._manager;
            set => this._manager = value;
        }

        void IPriorityState.OnEnter(IPrioritizer manager) {
            this.onEnter?.Invoke(manager);
            this.OnEnter(manager);
        }

        void IPriorityState.OnRefresh(IPrioritizer manager) {
            this.onRefresh?.Invoke(manager);
            this.OnRefresh(manager);
        }

        void IPriorityState.OnEnable(IPrioritizer manager) {
            this.onEnable?.Invoke(manager);
            this.OnEnable(manager);
        }

        void IPriorityState.OnUpdate(IPrioritizer manager, float deltaTime) {
            this.onUpdate?.Invoke(manager, deltaTime);
            this.OnUpdate(manager, deltaTime);
        }

        void IPriorityState.OnDisable(IPrioritizer manager) {
            this.onDisable?.Invoke(manager);
            this.OnDisable(manager);
        }

        void IPriorityState.OnLeave(IPrioritizer manager) {
            this.onLeave?.Invoke(manager);
            this.OnLeave(manager);
        }

        void IPriorityState.OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) {
            this.onLeaveDetails?.Invoke(manager, leaveDetails);
            this.OnLeaveDetails(manager, leaveDetails);
        }

        [MemoryPackIgnore]
        public Action<IPrioritizer> onEnter;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onRefresh;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onEnable;

        [MemoryPackIgnore]
        public Action<IPrioritizer, float> onUpdate;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onDisable;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onLeave;

        [MemoryPackIgnore]
        public Action<IPrioritizer, PriorityStateLeaveDetails> onLeaveDetails;

        /// <summary>
        /// 是否在管理器里
        /// </summary>
        [MemoryPackIgnore]
        public bool IsEntered => this._manager != null;

        protected virtual void OnEnter(IPrioritizer manager) { }

        protected virtual void OnRefresh(IPrioritizer manager) { }

        protected virtual void OnEnable(IPrioritizer manager) { }

        protected virtual void OnUpdate(IPrioritizer manager, float deltaTime) { }

        protected virtual void OnDisable(IPrioritizer manager) { }

        protected virtual void OnLeave(IPrioritizer manager) { }

        protected virtual void OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) { }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool EnterState(PriorityStateEnterFailDetails details = null) {
            if (this._targetPrioritizer == null) {
                if (details != null) {
                    details.FailType = PriorityStateEnterFailType.PrioritiesIsNull;
                }

                return false;
            }

            if (this.IsEntered && !this.allowReenter) {
                if (details != null) {
                    details.FailType = PriorityStateEnterFailType.NoReentry;
                }

                return false;
            }

            return ((IPriorityState)this).Enter(this._targetPrioritizer, details);
        }

        /// <summary>
        /// 测试是否能进入，但不真的进入
        /// </summary>
        /// <param name="successCache">如果成功, 则缓存该状态, 在优先器内的任何状态发生改变之前, 该状态进入时, 不需要再次对比检查, 使用前要自行确定这期间, 不会有影响公正性的改变</param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool EvaluateState(bool successCache = false, PriorityStateEnterFailDetails details = null) {
            if (this._targetPrioritizer == null) {
                if (details != null) {
                    details.FailType = PriorityStateEnterFailType.PrioritiesIsNull;
                }

                return false;
            }
            
            if (this.IsEntered && !this.allowReenter) {
                if (details != null) {
                    details.FailType = PriorityStateEnterFailType.NoReentry;
                }

                return false;
            }

            return ((IPriorityState)this).Evaluate(this._targetPrioritizer, successCache, details);
        }

        /// <summary>
        /// 刷新, 会重置时间, 并且触发一次刷新回调
        /// </summary>
        /// <returns></returns>
        public bool RefreshState() => ((IPriorityState)this).Refresh();

        /// <summary>
        /// 离开状态机
        /// </summary>
        /// <returns></returns>
        public bool LeaveState() => ((IPriorityState)this).Leave();
    }
}