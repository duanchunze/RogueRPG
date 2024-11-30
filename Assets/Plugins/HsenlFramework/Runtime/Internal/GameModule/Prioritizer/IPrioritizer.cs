using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hsenl {
    /*
     * 优先级处理系统
     *
     * 类似无限状态机, 状态个数不确定, 可自行退出
     * 基于优先级与通道
     * 声明为接口, 并写好了部分逻辑(internal), 继承者自行决定是否实现. 接口更像是一个写好了完整逻辑的模板, 但不会强迫子类继承字段, 也不会强迫继承全部函数
     * 这里只是第一层, 还需要实现一个第二层, 来自行决定是否要屏蔽掉某些函数
     * 最终由其他程序集里去使用
     */
    public interface IPrioritizer {
        public float TimeScale { get; set; }
        protected HashSet<IPriorityState> _allStates { get; }
        protected Dictionary<int, Aisle> _aisles { get; }
        public IPriorityState CurrentEnterState { get; set; }
        public IPriorityState EvaluateSuccessdCache { get; protected set; } // 评估成功的状态, 在优先器发生变化之前, 如果该状态进入, 则不需要对比了
        protected Queue<int> _dirtyFlags { get; }
        protected Dictionary<int, IPriorityState> _defaults { get; }

        // 临时缓存, 继承者必须提供
        protected List<IPriorityState> _removesCache { get; }
        protected Stack<IPriorityState> _pollingCache { get; } // 用于轮询的临时缓存

        /// <summary>
        /// 设置一个默认的行为，当该通道没有行为的时候，会使用该默认行为，一般使用在主通道里
        /// </summary>
        /// <param name="aisleId">一般使用0代表主通道</param>
        /// <param name="state"></param>
        public void SetDefaultState(int aisleId, IPriorityState state) {
            if (this._defaults == null) return;
            this._defaults[aisleId] = state;
            if (!this._aisles.TryGetValue(aisleId, out var aisle)) {
                state.Enter_Interface(this);
                return;
            }

            if (aisle.states.Count == 0) {
                state.Enter_Interface(this);
            }
        }

        public void SetDefaultState(IPriorityState state) {
            foreach (var aisle in state.aisles) {
                this.SetDefaultState(aisle, state);
            }
        }

        public IPriorityState GetDefaultState(int aisleId) {
            if (this._defaults == null) return null;
            if (this._defaults.TryGetValue(aisleId, out var result)) {
                return result;
            }

            return null;
        }

        public void RemoveDefaultState(int aisleId, IPriorityState state) {
            if (this._defaults == null) return;
            if (!this._defaults.TryGetValue(aisleId, out var result)) return;
            if (result != state) return;
            this._defaults.Remove(aisleId);
        }

        public void RemoveDefaultState(IPriorityState state) {
            foreach (var aisle in state.aisles) {
                this.RemoveDefaultState(aisle, state);
            }
        }

        public bool Contains(string name) {
            foreach (var state in this._allStates) {
                if (state.Name == name)
                    return true;
            }

            return false;
        }

        public IPriorityState Get(string name) {
            foreach (var state in this._allStates) {
                if (state.Name == name)
                    return state;
            }

            return null;
        }

        internal bool Evaluate(IPriorityState state, bool successCache, out PriorityStateEnterDetails details) {
            details = default;
            if (state.Manager != null) {
                if (state.Manager != this) {
                    Log.Error($"<一个状态同时只能进入一个状态机> '{state.Name}'");
                    return false;
                }
            }

            if (this.CheckEnter(state, ref details)) {
                if (successCache)
                    this.EvaluateSuccessdCache = state;

                return true;
            }

            return false;
        }

        internal bool Enter(IPriorityState state, out PriorityStateEnterDetails details) {
            details = default;
            if (state.Manager != null) {
                if (state.Manager != this) {
                    throw new Exception($"<一个状态同时只能进入一个状态机> '{state.Name}'");
                }
            }

            if (state == this.EvaluateSuccessdCache || this.CheckEnter(state, ref details)) {
                if (state.IsEntered_Interface)
                    this.Remove(state, PriorityStateLeaveType.ReEnter, state);

                // 先把状态加入
                this.Add(state);
                // 然后把它能排挤掉的状态排挤掉
                this.CheckExclusion(state);
                state.OnEnter_Internal(this);
                if (!state.IsEntered_Interface) {
                    return true;
                }

                try {
                    this.OnStateChanged(state, true);
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                return true;
            }

            return false;
        }

        internal bool Leave(IPriorityState state) {
            if (state == null) {
                Log.Error("状态为空!");
                return false;
            }

            if (!state.IsEntered_Interface) {
                return false;
            }

            this.Remove(state, PriorityStateLeaveType.InitiativeInvoke, null);
            return true;
        }

        private void Add(IPriorityState state) {
            this._allStates.Add(state);
            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (!this._aisles.TryGetValue(aisleId, out var aisle)) {
                    aisle = new Aisle();
                    this._aisles[aisleId] = aisle;
                }

                aisle.AddState(state);
                this.FlagDirty(aisleId);
            }

            state.Manager = this;
            this.CurrentEnterState = state;
            this.EvaluateSuccessdCache = null;
        }

        private void Remove(IPriorityState state, PriorityStateLeaveType leaveType, IPriorityState initiator) {
            if (!this._allStates.Remove(state))
                return;

            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    if (!aisle.RemoveState(state)) {
                        Log.Error($"<从通道中移除状态出现异常，通道不包含该状态> 异常状态:'{state.Name}'，通道名称：'{aisleId}'");
                    }
                }
                else {
                    Log.Error($"<从通道中移除状态出现异常，不包含该通道> 异常状态：'{state.Name}'，通道名称：'{aisleId}'");
                }

                this.FlagDirty(aisleId);
            }

            state.Manager = null;
            if (this.CurrentEnterState == state) {
                this.CurrentEnterState = null;
            }

            this.EvaluateSuccessdCache = null;

            state.OnLeave_Internal(this);
            state.OnLeaveDetails_Internal(this, new PriorityStateLeaveDetails { LeaveType = leaveType, Initiator = initiator });
            try {
                this.OnStateChanged(state, false);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void RecheckKeep(IPriorityState state) {
            var aisleIds = state.aisles;
            var multiAisle = aisleIds.Count > 1;
            if (multiAisle) this.ResetHandledTokens();
            IPriorityState initiator = null;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (!this._aisles.TryGetValue(aisleId, out var aisle)) {
                    continue;
                }

                foreach (var other in aisle.states) {
                    // 如果该状态已经被排挤过了
                    if (multiAisle && other.handledFlag) continue;

                    if (this.ContrastExclusion(state, other)) {
                        initiator = other;
                        goto REMOVE;
                    }

                    if (multiAisle) other.handledFlag = true;
                }
            }

            return;

            REMOVE:
            this.Remove(state, PriorityStateLeaveType.Exclusion, initiator);
        }

        // 当state的exclusion变化时, 可以调用该方法进行重检
        internal void RecheckExclusion(IPriorityState state) {
            if (!state.IsEntered_Interface)
                return;

            this.CheckExclusion(state);
        }

        // 当state的run变化时, 可以调用该方法进行重检
        internal void RecheckRun(IPriorityState state) {
            if (!state.IsEntered_Interface)
                return;

            this.FlagDirty(state);
        }

        internal void RefreshObstructPriority(IPriorityState state, bool isUp) {
            if (!state.IsEntered_Interface)
                return;

            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    if (isUp) {
                        aisle.ObstructUp(state);
                    }
                    else {
                        aisle.ObstructDown(state);
                    }
                }
            }
        }

        internal void RefreshDisablePriority(IPriorityState state, bool isUp) {
            if (!state.IsEntered_Interface)
                return;

            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    if (isUp) {
                        aisle.DisableUp(state);
                    }
                    else {
                        aisle.DisableDown(state);
                    }

                    this.FlagDirty(aisleId);
                }
            }
        }

        internal void RefreshObstructLabels(IPriorityState state, bool isUp) {
            if (!state.IsEntered_Interface)
                return;

            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    if (isUp) {
                        aisle.AddSpecialObstructLabels(state);
                    }
                    else {
                        aisle.RemoveSpecialObstructLabels(state);
                    }
                }
            }
        }

        internal void RefreshDisableLabels(IPriorityState state, bool isUp) {
            if (!state.IsEntered_Interface)
                return;

            var aisleIds = state.aisles;
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    if (isUp) {
                        aisle.AddSpecialDisableLabels(state);
                    }
                    else {
                        aisle.RemoveSpecialDisableLabels(state);
                    }

                    this.FlagDirty(aisleId);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FlagDirty(int aisleId) {
            if (!this._dirtyFlags.Contains(aisleId)) {
                this._dirtyFlags.Enqueue(aisleId);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FlagDirty(IPriorityState state) {
            for (int i = 0, len = state.aisles.Count; i < len; i++) {
                this.FlagDirty(state.aisles[i]);
            }
        }

        /// <summary>
        /// 检查进入
        /// </summary>
        /// <param name="state"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        private bool CheckEnter(IPriorityState state, ref PriorityStateEnterDetails details) {
            // 遍历所有当前状态想要进入的通道，并和通道里的所有状态一一对比，全部通过，才判定为可以进入。
            var aisleIds = state.aisles;
            var multiAisle = aisleIds.Count > 1;
            var reseted = !multiAisle;
            // 遍历要进入的状态的通道
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                // 如果该状态机里没有该通道，则可以认为该状态是该通道的第一个，那必然可进入
                if (!this._aisles.TryGetValue(aisleId, out var aisle)) {
                    continue;
                }

                var stateCount = aisle.states.Count;
                if (aisle.TopContrastEnter(state.EnterPriority)) {
                    var b = aisle.ContainsAllSpecialObstructOfLabels(state.labels);
                    if (b) {
                        // 如果只有一个, 那肯定就是他拦的
                        if (stateCount == 1) {
                            details.FailType = PriorityStateEnterFailType.SpecialIntercept;
                            details.Blocker = aisle.states[0];
                            return false;
                        }

                        // 可能是别的拦截者, 逐个比对, 这里也可以直接返回 false, 但逐个对比能确定具体是哪个state拦截的
                        goto EVERYONE_CHECK;
                    }

                    // 大于最大拦截, 且没有指定拦截, 则直接判定可以进入
                    details.FailType = PriorityStateEnterFailType.PriorityOk;
                    continue;
                }
                else {
                    var b = aisle.TopObstructPriorityState.ContainsSpecialPassedOfLabels(state.labels);
                    if (b) {
                        if (stateCount == 1) {
                            details.FailType = PriorityStateEnterFailType.SpecialPass;
                            continue;
                        }

                        // 最大拦截者指定通过了, 剩下的就不能确定了, 逐个比对
                        goto EVERYONE_CHECK;
                    }

                    details.FailType = PriorityStateEnterFailType.PriorityLow;
                    details.Blocker = aisle.TopObstructPriorityState;
                    return false;
                }

                EVERYONE_CHECK:
                ResetHandled();
                // 遍历该通道里所有的状态
                foreach (var other in aisle.states) {
                    // 如果该状态已经存在于临时区，则继续
                    if (multiAisle && other.handledFlag) continue;

                    // 获得这个对比者，用它和要进入的状态做对比
                    if (!this.ContrastEnter(state, other, ref details)) {
                        return false;
                    }

                    // 把对比者加入缓存区，之后便不需要再次和它对比
                    if (multiAisle) other.handledFlag = true;
                }
            }

            // 如果enter小于0, 那么即使通道是空的, 也不能进入.
            if (state.EnterPriority < 0) {
                if (details.FailType == PriorityStateEnterFailType.NoContraster) {
                    details.FailType = PriorityStateEnterFailType.PriorityLow;
                    details.Blocker = null;
                    return false;
                }
            }

            return true;

            void ResetHandled() {
                if (!reseted) {
                    reseted = true;
                    this.ResetHandledTokens();
                }
            }
        }

        /// <summary>
        /// 对比状态进入相关
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contrast"></param>
        /// <param name="details"></param>
        /// <returns>true 代表 source 相对于 contrast 可以进入</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ContrastEnter(IPriorityState source, IPriorityState contrast, ref PriorityStateEnterDetails details) {
            // 进入优先级 > 对比者
            if (source.EnterPriority >= contrast.ObstructPriority) {
                // 如果被对比者点名拦截或者点名标签拦截，则依然返回 false
                var pass = !contrast.ContainsSpecialObstructOfLabels(source.labels);
                if (!pass) {
                    details.FailType = PriorityStateEnterFailType.SpecialIntercept;
                    details.Blocker = contrast;
                    return false;
                }

                details.FailType = PriorityStateEnterFailType.PriorityOk;
            }
            else {
                // 如果被对比者点名通过或者点名标签通过，则依然返回 true
                var pass = contrast.ContainsSpecialPassedOfLabels(source.labels);
                if (!pass) {
                    details.FailType = PriorityStateEnterFailType.PriorityLow;
                    details.Blocker = contrast;
                    return false;
                }

                details.FailType = PriorityStateEnterFailType.SpecialPass;
            }

            return true;
        }

        /// <summary>
        /// 检查排挤
        /// </summary>
        /// <param name="state"></param>
        private void CheckExclusion(IPriorityState state) {
            // 遍历对比者要对比的所有通道，和每个通道里的所有状态做对比，把能排挤掉的都排挤掉
            var aisleIds = state.aisles;
            var multiAisle = aisleIds.Count > 1;
            this._removesCache.Clear();
            if (multiAisle) this.ResetHandledTokens();
            // 遍历对比者要求的所有通道
            for (int i = 0, len = aisleIds.Count; i < len; i++) {
                var aisleId = aisleIds[i];
                if (!this._aisles.TryGetValue(aisleId, out var aisle)) {
                    // 没有对应的通道很正常，状态机创建一开始，通道是空的，会根据进入的状态来拓展通道
                    continue;
                }

                // 遍历通道里的每个状态
                foreach (var other in aisle.states) {
                    // 如果该状态已经被排挤过了
                    if (multiAisle && other.handledFlag) continue;

                    if (this.ContrastExclusion(other, state)) {
                        this._removesCache.Add(other);
                    }

                    if (multiAisle) other.handledFlag = true;
                }
            }

            for (int i = 0, len = this._removesCache.Count; i < len; i++) {
                this.Remove(this._removesCache[i], PriorityStateLeaveType.Exclusion, state);
            }
        }

        /// <summary>
        /// 对比状态排挤相关
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contrast"></param>
        /// <returns>true 代表 source 会被 contrast 排挤掉</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ContrastExclusion(IPriorityState source, IPriorityState contrast) {
            // 自己不会排挤自己
            if (source == contrast) {
                return false;
            }

            // 自己的保持优先级 >= 对比者的排挤优先级, 代表 source 不会被排挤掉
            if (source.KeepPriority >= contrast.ExclusionPriority) {
                // 如果自己被对比者点名或标签排挤，则依然返回 true
                return contrast.ContainsSpecialExclusionOfLabels(source.labels);
            }
            else {
                // 如果自己被对比者点名或标签保持，则依然返回 false
                return !contrast.ContainsSpecialKeepOfLabels(source.labels);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetHandledTokens() {
            foreach (var state in this._allStates) {
                state.handledFlag = false;
            }
        }

        protected void OnStateChanged(IPriorityState state, bool isEnter);

        // 外部调用, 以正常运行
        public void Update(float deltaTime) {
            if (this._dirtyFlags.Count > 0) {
                this.HandleDirtiness();
            }

            // timeScale 不允许为小于0，不然倒着跑了
            if (this.TimeScale < 0) {
                this.TimeScale = 0;
            }

            var finalDeltaTime = deltaTime * this.TimeScale;

            this._pollingCache.Clear();
            foreach (var state in this._allStates) {
                this._pollingCache.Push(state);
            }

            while (this._pollingCache.Count != 0) {
                var state = this._pollingCache.Pop();
                if (!state.IsEntered_Interface) {
                    // 说明该state被上一轮run的过程中执行的某些逻辑给删除了
                    continue;
                }

                byte ret = 0;
                try {
                    ret = state.Run(finalDeltaTime);
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                switch (ret) {
                    case 0: // 代表正常走
                        continue;
                    case 1: // 结束，移除该状态
                        this.Remove(state, PriorityStateLeaveType.TimeOut, null);
                        break;
                }
            }
        }

        /// <summary>
        /// 1、刷新禁用
        /// 2、补充空通道
        /// </summary>
        private void HandleDirtiness() {
            // 把count单独出来，该帧内再出现的新的脏数据，顺留到下帧处理，防止出现死循环的可能
            var dirtyCount = this._dirtyFlags.Count;
            while (dirtyCount > 0) {
                dirtyCount--;
                var aisleId = this._dirtyFlags.Dequeue();

                // 当运行 >= 禁用的时候，就可以运行
                // 只要被特别禁用，那么必定会被禁用
                // 所有禁用权限比你大的人，只要有一个没有指定让你运行，都不可以运行
                // 特别禁用的优先级比特别运行的优先级高
                // 运行与禁用的等级最低都是0，而禁用0也代表着该状态不想禁用其他状态
                // 从这些改变过的通道里，遍历找出最大的禁用状态
                if (this._aisles.TryGetValue(aisleId, out var aisle)) {
                    var states = aisle.states;
                    switch (states.Count) {
                        case > 1: {
                            foreach (var state in states) {
                                // 先把所有状态的暂停关闭。
                                state.Paused = false;
                            }

                            foreach (var state in states) {
                                // 如果该状态已经暂停了，说明它已经在其他通道被暂停过了，所以就不需要再次判断了
                                if (state.Paused) {
                                    continue;
                                }

                                // 首先判断是不是该状态是不是被特别指定要禁用的
                                if (aisle.ContainsAllSpecialDisableOfLabels(state.labels)) {
                                    state.Paused = true;
                                    continue;
                                }

                                // 如果运行等级比最高禁用等级，则直接跳过
                                if (!aisle.TopContrastDisable(state.RunPriority)) {
                                    continue;
                                }

                                // 否则就和每一个禁用者对比
                                foreach (var comparer in states) {
                                    // 自己不会禁用自己
                                    if (comparer == state) {
                                        continue;
                                    }

                                    // 一旦有一个没比过，且也没被对方指定运行，则直接暂停并中断比较
                                    if (state.RunPriority < comparer.DisablePriority) {
                                        if (!comparer.ContainsSpecialRunOfLabels(state.labels)) {
                                            state.Paused = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            break;
                        }
                        // 如果该通道是空的，看是否有默认状态可以补充该通道
                        case 0: {
                            if (this._defaults != null) {
                                if (this._defaults.TryGetValue(aisleId, out var defaultState)) {
                                    defaultState.Enter_Interface(this);
                                }
                            }

                            break;
                        }
                    }
                }
                else {
                    Log.Error($"<虽然记录通道发生了改变，但并没有找到该通道> {aisleId}");
                }
            }
        }

        public class Aisle {
            public readonly List<IPriorityState> states = new();

            public IPriorityState TopObstructPriorityState { get; private set; }
            public IPriorityState TopDisablePriorityState { get; private set; }
            private Bitlist _specialObstructLabels;
            private Bitlist _specialDisableLabels;

            private bool _obstructDirty;
            private bool _disableDirty;

            public bool AddState(IPriorityState state) {
                this.states.Add(state);
                this.ObstructUp(state);
                this.DisableUp(state);
                this.AddSpecialObstructLabels(state);
                this.AddSpecialDisableLabels(state);
                return true;
            }

            public bool RemoveState(IPriorityState state) {
                var ret = this.states.Remove(state);
                this.ObstructDown(state);
                this.DisableDown(state);
                if (!Bitlist.IsNullOrEmpty(state.specialObstructLabels))
                    this.RemoveSpecialObstructLabels(state);
                if (!Bitlist.IsNullOrEmpty(state.specialDisableLabels))
                    this.RemoveSpecialDisableLabels(state);
                return ret;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ObstructUp(IPriorityState state) {
                if (this.TopObstructPriorityState == null) {
                    this.TopObstructPriorityState = state;
                    return true;
                }

                if (state.ObstructPriority > this.TopObstructPriorityState.ObstructPriority) {
                    this.TopObstructPriorityState = state;
                    return true;
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ObstructDown(IPriorityState state) {
                if (state != this.TopObstructPriorityState)
                    return false;

                this.TopObstructPriorityState = null;
                this._obstructDirty = true;
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool DisableUp(IPriorityState state) {
                if (this.TopDisablePriorityState == null) {
                    this.TopDisablePriorityState = state;
                    return true;
                }

                if (state.DisablePriority > this.TopDisablePriorityState.DisablePriority) {
                    this.TopDisablePriorityState = state;
                    return true;
                }

                return false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool DisableDown(IPriorityState state) {
                if (state != this.TopDisablePriorityState)
                    return false;

                this.TopDisablePriorityState = null;
                this._disableDirty = true;
                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TopContrastEnter(int enterPriority) {
                if (this._obstructDirty)
                    this.RefreshObstruct();

                if (this.TopObstructPriorityState == null)
                    return true;

                return enterPriority >= this.TopObstructPriorityState.ObstructPriority;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool TopContrastDisable(int runPriority) {
                if (this._disableDirty)
                    this.RefreshDisable();

                if (this.TopDisablePriorityState == null)
                    return false;

                return this.TopDisablePriorityState.DisablePriority > runPriority;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsAllSpecialObstructOfLabels(Bitlist lbs) {
                if (this._obstructDirty)
                    this.RefreshObstruct();

                return this._specialObstructLabels?.ContainsAny(lbs) ?? false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool ContainsAllSpecialDisableOfLabels(Bitlist lbs) {
                if (this._disableDirty)
                    this.RefreshDisable();

                return this._specialDisableLabels?.ContainsAny(lbs) ?? false;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddSpecialObstructLabels(IPriorityState state) {
                if (Bitlist.IsNullOrEmpty(state.specialObstructLabels))
                    return;

                this._specialObstructLabels ??= new();
                this._specialObstructLabels.Add(state.specialObstructLabels);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void AddSpecialDisableLabels(IPriorityState state) {
                if (Bitlist.IsNullOrEmpty(state.specialDisableLabels))
                    return;

                this._specialDisableLabels ??= new();
                this._specialDisableLabels.Add(state.specialDisableLabels);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void RemoveSpecialObstructLabels(IPriorityState state) {
                this._obstructDirty = true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void RemoveSpecialDisableLabels(IPriorityState state) {
                this._disableDirty = true;
            }

            private void RefreshObstruct() {
                this.TopObstructPriorityState = null;
                this._specialObstructLabels?.Clear();
                for (int i = 0, len = this.states.Count; i < len; i++) {
                    var state = this.states[i];
                    this.ObstructUp(state);
                    this.AddSpecialObstructLabels(state);
                }
            }

            private void RefreshDisable() {
                this.TopDisablePriorityState = null;
                this._specialDisableLabels?.Clear();
                for (int i = 0, len = this.states.Count; i < len; i++) {
                    var state = this.states[i];
                    this.DisableUp(state);
                    this.AddSpecialDisableLabels(state);
                }
            }
        }
    }
}