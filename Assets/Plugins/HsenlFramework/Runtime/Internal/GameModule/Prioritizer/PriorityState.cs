using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
    public partial class PriorityState : Unbodied, IPriorityState {
        #region 提供给接口

        private bool _handledFlag;

        /// 该时间缩放应该用在游戏逻辑上, 而不是用在比如击败boss的慢动作上, 那个应该用TimeInfo.TimeScale
#if UNITY_EDITOR
        [SerializeField, PropertyRange(0f, 5f), LabelText("时间缩放"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private float _timeScale = 1f;

#if UNITY_EDITOR
        [SerializeField, LabelText("通道"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int[] _aisles = { 0 };

        [MemoryPackInclude]
        private int _enterPriorityAnchor;

#if UNITY_EDITOR
        [SerializeField, LabelText("进入等级"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _enterPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("阻拦等级锚点"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _obstructPriorityAnchor;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("阻拦等级"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _obstructPriority;

        [MemoryPackInclude]
        private int _exclusionPriorityAnchor;

#if UNITY_EDITOR
        [SerializeField, LabelText("排挤等级"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _exclusionPriority;

        [MemoryPackInclude]
        private int _keepPriorityAnchor;

#if UNITY_EDITOR
        [SerializeField, LabelText("保持等级"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _keepPriority;

        [MemoryPackInclude]
        private int _disablePriorityAnchor;

#if UNITY_EDITOR
        [SerializeField, LabelText("禁用等级"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _disablePriority;

        [MemoryPackInclude]
        private int _runPriorityAnchor;

#if UNITY_EDITOR
        [SerializeField, LabelText("运行等级"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private int _runPriority;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定通过"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialPassLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定拦截"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialInterceptLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定排挤"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialExclusionLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定保持"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialKeepLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定禁用"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialDisableLabels;

#if UNITY_EDITOR
        [SerializeField, LabelText("指定运行"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private Bitlist _specialRunLabels;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("上一帧暂停"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private bool _pausedPrevious;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("暂停"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private bool _paused;

#if UNITY_EDITOR
        [SerializeField, ReadOnly, LabelText("持续时间"), FoldoutGroup("优先级")]
#endif
        [MemoryPackInclude]
        private float _duration;

#if UNITY_EDITOR
        [SerializeField, ReadOnly, LabelText("当前时间"), FoldoutGroup("优先级")]
#endif
        private float _time;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("时间暂停"), DisableInEditorMode, FoldoutGroup("优先级")]
#endif
        public bool timeParse;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("状态管理器"), FoldoutGroup("优先级")]
#endif
        private IPrioritizer _manager;

        bool IPriorityState.handledFlag {
            get => this._handledFlag;
            set => this._handledFlag = value;
        }

        float IPriorityState.TimeScale {
            get => this._timeScale;
        }

        string IPriorityState.Name {
            get => this.Entity.name;
        }

        IList<int> IPriorityState.aisles {
            get => this._aisles;
            set => this._aisles = value.ToArray();
        }

        Bitlist IPriorityState.labels {
            get => this.Tags;
        }

        int IPriorityState.EnterPriorityAnchor {
            get => this._enterPriorityAnchor;
            set => this._enterPriorityAnchor = value;
        }

        int IPriorityState.EnterPriority {
            get => this._enterPriority;
            set => this._enterPriority = value;
        }

        int IPriorityState.ObstructPriorityAnchor {
            get => this._obstructPriorityAnchor;
            set => this._obstructPriorityAnchor = value;
        }

        int IPriorityState.ObstructPriority {
            get => this._obstructPriority;
            set => this._obstructPriority = value;
        }

        int IPriorityState.KeepPriorityAnchor {
            get => this._keepPriorityAnchor;
            set => this._keepPriorityAnchor = value;
        }

        int IPriorityState.ExclusionPriorityAnchor {
            get => this._exclusionPriorityAnchor;
            set => this._exclusionPriorityAnchor = value;
        }

        int IPriorityState.ExclusionPriority {
            get => this._exclusionPriority;
            set => this._exclusionPriority = value;
        }

        int IPriorityState.RunPriorityAnchor {
            get => this._runPriorityAnchor;
            set => this._runPriorityAnchor = value;
        }

        int IPriorityState.KeepPriority {
            get => this._keepPriority;
            set => this._keepPriority = value;
        }

        int IPriorityState.DisablePriorityAnchor {
            get => this._disablePriorityAnchor;
            set => this._disablePriorityAnchor = value;
        }

        int IPriorityState.DisablePriority {
            get => this._disablePriority;
            set => this._disablePriority = value;
        }

        int IPriorityState.RunPriority {
            get => this._runPriority;
            set => this._runPriority = value;
        }

        Bitlist IPriorityState._specialPassLabels {
            get => this._specialPassLabels;
            set => this._specialPassLabels = value;
        }

        Bitlist IPriorityState.specialObstructLabels {
            get => this._specialInterceptLabels;
            set => this._specialInterceptLabels = value;
        }

        Bitlist IPriorityState._specialExclusionLabels {
            get => this._specialExclusionLabels;
            set => this._specialExclusionLabels = value;
        }

        Bitlist IPriorityState._specialKeepLabels {
            get => this._specialKeepLabels;
            set => this._specialKeepLabels = value;
        }

        Bitlist IPriorityState.specialDisableLabels {
            get => this._specialDisableLabels;
            set => this._specialDisableLabels = value;
        }

        Bitlist IPriorityState._specialRunLabels {
            get => this._specialRunLabels;
            set => this._specialRunLabels = value;
        }

        bool IPriorityState._pausedPrevious {
            get => this._pausedPrevious;
            set => this._pausedPrevious = value;
        }

        bool IPriorityState.Paused {
            get => this._paused;
            set => this._paused = value;
        }

        float IPriorityState.Duration {
            get => this._duration;
        }

        float IPriorityState.Time {
            get => this._time;
            set => this._time = value;
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

        #endregion

#if UNITY_EDITOR
        [LabelText("允许重进"), FoldoutGroup("优先级")]
#endif
        public bool allowReenter;

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

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        private IPrioritizer _targetPrioritizer;

        [MemoryPackIgnore]
        public float TimeScale {
            get => this._timeScale;
            set => this._timeScale = value;
        }
        
        [MemoryPackIgnore]
        public int ObstructPriorityAnchor => this._obstructPriorityAnchor;

        [MemoryPackIgnore]
        public int ObstructPriority {
            get => this._obstructPriority;
            set => ((IPriorityState)this).ModifyObstruct_Interface(value);
        }

        [MemoryPackIgnore]
        public float Duration {
            get => this._duration;
            set => this._duration = value;
        }

        /// <summary>
        /// 是否在管理器里
        /// </summary>
        [MemoryPackIgnore]
        public bool IsEntered => ((IPriorityState)this).IsEntered_Interface;

        [MemoryPackIgnore]
        public IPrioritizer TargetPrioritizer {
            set {
                this._targetPrioritizer = value;
                if (this._targetPrioritizer == null) {
                    this.Leave();
                }
            }
        }

        internal override void OnDestroyInternal() {
            this.Leave();
        }

        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            this._handledFlag = false;
            this._timeScale = 1f;
            this._aisles = null;
            this._enterPriorityAnchor = 0;
            this._enterPriority = 0;
            this._obstructPriorityAnchor = 0;
            this._obstructPriority = 0;
            this._keepPriorityAnchor = 0;
            this._keepPriority = 0;
            this._exclusionPriorityAnchor = 0;
            this._exclusionPriority = 0;
            this._runPriorityAnchor = 0;
            this._runPriority = 0;
            this._disablePriority = 0;
            this._specialPassLabels?.Clear();
            this._specialInterceptLabels?.Clear();
            this._specialExclusionLabels?.Clear();
            this._specialKeepLabels?.Clear();
            this._specialDisableLabels?.Clear();
            this._specialRunLabels?.Clear();
            this._pausedPrevious = false;
            this._paused = false;
            this._duration = 0;
            this._time = 0;
            this.timeParse = false;
            this._manager = null;
            this.allowReenter = false;
            this.TargetPrioritizer = null;
        }

        public void InitAisles(IList<int> aisleIds) => ((IPriorityState)this).InitAisles_Interface(aisleIds);

        public void InitPriorities(int enter, int obstruct, int keep, int exclusion, int run, int disable) =>
            ((IPriorityState)this).InitPriorities_Interface(enter, obstruct, keep, exclusion, run, disable);

        /// <summary>
        /// 测试是否能进入，但不真的进入
        /// </summary>
        /// <param name="successCache">如果成功, 则缓存该状态, 在优先器内的任何状态发生改变之前, 该状态进入时, 不需要再次对比检查, 使用前要自行确定这期间, 不会有影响公正性的改变</param>
        /// <returns></returns>
        public bool Evaluate(bool successCache = false) {
            if (this._targetPrioritizer == null) {
                return false;
            }

            if (this.IsEntered && !this.allowReenter) {
                return false;
            }

            return ((IPriorityState)this).Evaluate_Interface(this._targetPrioritizer, successCache);
        }

        public bool Evaluate(out PriorityStateEnterDetails details, bool successCache = false) {
            details = default;
            if (this._targetPrioritizer == null) {
                details.FailType = PriorityStateEnterFailType.PrioritiesIsNull;
                return false;
            }

            if (this.IsEntered && !this.allowReenter) {
                details.FailType = PriorityStateEnterFailType.AlreadyExits;
                return false;
            }

            return ((IPriorityState)this).Evaluate_Interface(this._targetPrioritizer, out details, successCache);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <returns></returns>
        public bool Enter() {
            if (this._targetPrioritizer == null) {
                return false;
            }

            if (this.IsEntered && !this.allowReenter) {
                return false;
            }

            return ((IPriorityState)this).Enter_Interface(this._targetPrioritizer);
        }

        public bool Enter(out PriorityStateEnterDetails details) {
            details = default;
            if (this._targetPrioritizer == null) {
                details.FailType = PriorityStateEnterFailType.PrioritiesIsNull;
                return false;
            }

            if (this.IsEntered && !this.allowReenter) {
                details.FailType = PriorityStateEnterFailType.AlreadyExits;
                return false;
            }

            return ((IPriorityState)this).Enter_Interface(this._targetPrioritizer, out details);
        }

        /// <summary>
        /// 刷新, 会重置时间, 并且触发一次刷新回调
        /// </summary>
        /// <returns></returns>
        public bool Refresh() => ((IPriorityState)this).Refresh_Interface();

        /// <summary>
        /// 离开状态
        /// </summary>
        /// <returns></returns>
        public bool Leave() => ((IPriorityState)this).Leave_Interface();

        protected virtual void OnEnter(IPrioritizer manager) { }

        protected virtual void OnRefresh(IPrioritizer manager) { }

        protected virtual void OnEnable(IPrioritizer manager) { }

        protected virtual void OnUpdate(IPrioritizer manager, float deltaTime) { }

        protected virtual void OnDisable(IPrioritizer manager) { }

        protected virtual void OnLeave(IPrioritizer manager) { }

        protected virtual void OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) { }
    }
}