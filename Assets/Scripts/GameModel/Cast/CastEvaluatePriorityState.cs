using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;

// using UnityEngine.Serialization;
// #if UNITY_EDITOR
// using Sirenix.OdinInspector;
// using UnityEngine;
// #endif

namespace Hsenl {
    [MemoryPackable]
    public sealed partial class CastEvaluatePriorityState : Unbodied, IPriorityState {
        #region 提供给接口

        private bool _handledFlag;

        [MemoryPackInclude]
        private int[] _aisles = { 0 };

        [MemoryPackInclude]
        private int _enterPriority;

        [MemoryPackInclude]
        private int _obstructPriority;

        [MemoryPackInclude]
        private int _exclusionPriority;

        [MemoryPackInclude]
        private int _keepPriority;

        // [MemoryPackInclude]
        // private int _disablePriority;
        //
        // [MemoryPackInclude]
        // private int _runPriority;

        // [MemoryPackInclude]
        // private Bitlist _specialPassLabels;
        //
        // [MemoryPackInclude]
        // private Bitlist _specialInterceptLabels;
        //
        // [MemoryPackInclude]
        // private Bitlist _specialExclusionLabels;

        [MemoryPackInclude]
        private Bitlist _specialKeepLabels;

        // [MemoryPackInclude]
        // private Bitlist _specialDisableLabels;
        //
        // [MemoryPackInclude]
        // private Bitlist _specialRunLabels;

        [MemoryPackInclude]
        private bool _pausedPrevious;

        [MemoryPackInclude]
        private bool _paused;

        [MemoryPackInclude]
        private float _duration;

        private float _time;

        public bool timeParse;

        private IPrioritizer _manager;

        bool IPriorityState.handledFlag {
            get => this._handledFlag;
            set => this._handledFlag = value;
        }

        float IPriorityState.TimeScale {
            get => 1f;
        }

        string IPriorityState.Name {
            get => this.Entity.Name;
        }

        IList<int> IPriorityState.aisles {
            get => this._aisles;
            set => this._aisles = value.ToArray();
        }

        Bitlist IPriorityState.labels {
            get => this.Tags;
        }

        int IPriorityState.EnterPriorityAnchor {
            get => this._enterPriority;
            set => this._enterPriority = value;
        }

        int IPriorityState.EnterPriority {
            get => this._enterPriority;
            set => this._enterPriority = value;
        }

        int IPriorityState.ObstructPriorityAnchor {
            get => this._obstructPriority;
            set => this._obstructPriority = value;
        }

        int IPriorityState.ObstructPriority {
            get => this._obstructPriority;
            set => this._obstructPriority = value;
        }

        int IPriorityState.KeepPriorityAnchor {
            get => this._keepPriority;
            set => this._keepPriority = value;
        }

        int IPriorityState.ExclusionPriorityAnchor {
            get => this._exclusionPriority;
            set => this._exclusionPriority = value;
        }

        int IPriorityState.ExclusionPriority {
            get => this._exclusionPriority;
            set => this._exclusionPriority = value;
        }

        int IPriorityState.RunPriorityAnchor {
            get => 0;
            set { }
        }

        int IPriorityState.KeepPriority {
            get => this._keepPriority;
            set => this._keepPriority = value;
        }

        int IPriorityState.DisablePriorityAnchor {
            get => 0;
            set { }
        }

        int IPriorityState.DisablePriority {
            get => 0;
            set { }
        }

        int IPriorityState.RunPriority {
            get => 0;
            set { }
        }

        Bitlist IPriorityState._specialPassLabels {
            get => null;
            set { }
        }

        Bitlist IPriorityState.specialObstructLabels {
            get => null;
            set { }
        }

        Bitlist IPriorityState._specialExclusionLabels {
            get => null;
            set { }
        }

        Bitlist IPriorityState._specialKeepLabels {
            get => this._specialKeepLabels;
            set => this._specialKeepLabels = value;
        }

        Bitlist IPriorityState.specialDisableLabels {
            get => null;
            set { }
        }

        Bitlist IPriorityState._specialRunLabels {
            get => null;
            set { }
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
        }

        void IPriorityState.OnRefresh(IPrioritizer manager) { }

        void IPriorityState.OnEnable(IPrioritizer manager) { }

        void IPriorityState.OnUpdate(IPrioritizer manager, float deltaTime) {
            this.onUpdate?.Invoke(manager, deltaTime);
        }

        void IPriorityState.OnDisable(IPrioritizer manager) { }

        void IPriorityState.OnLeave(IPrioritizer manager) {
            this.onLeave?.Invoke(manager);
        }

        void IPriorityState.OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) {
            this.onLeaveDetails?.Invoke(manager, leaveDetails);
        }

        #endregion

        public bool allowReenter;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onEnter;

        [MemoryPackIgnore]
        public Action<IPrioritizer, float> onUpdate;

        [MemoryPackIgnore]
        public Action<IPrioritizer> onLeave;

        [MemoryPackIgnore]
        public Action<IPrioritizer, PriorityStateLeaveDetails> onLeaveDetails;

        private IPrioritizer _targetPrioritizer;

        [MemoryPackIgnore]
        public float TimeScale {
            get => 1f;
            set { }
        }

        [MemoryPackIgnore]
        public int ObstructPriorityAnchor => this._obstructPriority;

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

        protected override void OnDestroy() {
            this.Leave();
        }

        protected override void OnDisposed() {
            base.OnDisposed();
            this._handledFlag = false;
            // this._timeScale = 1f;
            this._aisles = null;
            // this._enterPriorityAnchor = 0;
            this._enterPriority = 0;
            // this._obstructPriorityAnchor = 0;
            this._obstructPriority = 0;
            // this._keepPriorityAnchor = 0;
            this._keepPriority = 0;
            // this._exclusionPriorityAnchor = 0;
            this._exclusionPriority = 0;
            // this._runPriorityAnchor = 0;
            // this._runPriority = 0;
            // this._disablePriority = 0;
            // this._specialPassLabels?.Clear();
            // this._specialInterceptLabels?.Clear();
            // this._specialExclusionLabels?.Clear();
            this._specialKeepLabels?.Clear();
            // this._specialDisableLabels?.Clear();
            // this._specialRunLabels?.Clear();
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
    }
}