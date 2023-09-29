using System;
using System.Collections.Generic;
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
        protected HashSet<IPriorityState> States { get; }
        protected MultiList<int, IPriorityState> Aisles { get; }
        protected Queue<int> DirtyFlags { get; }
        protected IPriorityState CurrentEnterState { get; set; }
        protected IPriorityState EvaluateSuccessdCache { get; set; } // 评估成功的状态, 在优先器发生变化之前, 如果该状态进入, 则不需要对比了

        // 几个临时缓存
        protected List<IPriorityState> ExclusionsCache { get; }
        protected List<IPriorityState> HighestDisablesCache { get; }
        protected Bitlist SpecialDisableLabelsCache { get; }
        protected Stack<IPriorityState> PollingCache { get; } // 用于轮询的临时缓存
        protected Dictionary<int, IPriorityState> Defaults { get; }


        // /// <summary>
        // /// 细节判存
        // /// </summary>
        // /// <param name="state"></param>
        // /// <param name="same">名字相同, 或者完全相同</param>
        // /// <returns>0代表不存在, 1代表存在一个同名的但不是本人, 2代表存在且是本人</returns>
        // protected byte ContainsDetails(IPriorityState state, out IPriorityState same) {
        //     if (this.States.TryGetValue(state.Name, out same)) {
        //         if (state == same) {
        //             return 2;
        //         }
        //
        //         return 1;
        //     }
        //
        //     return 0;
        // }

        /// <summary>
        /// 1、刷新禁用
        /// 2、补充空通道
        /// </summary>
        protected void HandleDirtiness() {
            // 把count单独出来，该帧内再出现的新的脏数据，顺留到下帧处理，防止出现死循环的可能
            var dirtyCount = this.DirtyFlags.Count;
            while (dirtyCount > 0) {
                dirtyCount--;
                var aisle = this.DirtyFlags.Dequeue();

                #region 处理暂停相关

                // 当运行 >= 禁用的时候，就可以运行
                // 只要被特别禁用，那么必定会被禁用
                // 所有禁用权限比你大的人，只要有一个没有指定让你运行，都不可以运行
                // 特别禁用的优先级比特别运行的优先级高
                // 运行与禁用的等级最低都是0，而禁用0也代表着该状态不想禁用其他状态
                var maxDisablePriority = 0;
                this.HighestDisablesCache.Clear();
                this.SpecialDisableLabelsCache.Clear();
                // 从这些改变过的通道里，遍历找出最大的禁用状态
                if (this.Aisles.TryGetValue(aisle, out var list)) {
                    if (list.Count > 1) {
                        foreach (var state in list) {
                            // 先把所有状态的暂停关闭。
                            state.Paused = false;

                            // 把所有状态指定要禁用的标签都缓存下来
                            if (state.SpecialDisableLabels != null) {
                                this.SpecialDisableLabelsCache.Add(state.SpecialDisableLabels);
                            }

                            // 找出禁用优先级最大的状态
                            if (state.DisablePriority > maxDisablePriority) {
                                maxDisablePriority = state.DisablePriority;
                            }
                        }
                    }
                }
                else {
                    Log.Error($"<虽然记录通道发生了改变，但并没有找到该通道> {aisle}");
                    continue;
                }

                // 如果个数少于2个的话，就没必要比了
                if (list.Count > 1) {
                    // 把所有参与禁用的状态缓存起来，从大到小排好顺序
                    foreach (var state in list) {
                        // 0 表示该状态不想禁用任何其他状态
                        if (state.DisablePriority == 0) {
                            continue;
                        }

                        this.HighestDisablesCache.Add(state);
                    }

                    this.HighestDisablesCache.Sort((a, b) => b.DisablePriority.CompareTo(a.DisablePriority)); // 从大到小

                    foreach (var state in list) {
                        // 如果该状态已经暂停了，说明它已经在其他通道被暂停过了，所以就不需要再次判断了
                        if (state.Paused) {
                            continue;
                        }

                        // 首先判断是不是该状态是不是被特别指定要禁用的
                        if (this.SpecialDisableLabelsCache.ContainsAny(state.Labels)) {
                            state.Paused = true;
                            continue;
                        }

                        // 如果运行等级比最高禁用等级，则直接跳过
                        if (state.RunPriority >= maxDisablePriority) {
                            continue;
                        }

                        // 否则就和每一个禁用者对比
                        foreach (var comparer in this.HighestDisablesCache) {
                            // 自己不会禁用自己
                            if (comparer == state) {
                                continue;
                            }

                            // 一旦有一个没比过，且也没被对方指定运行，则直接暂停并中断比较
                            if (state.RunPriority < comparer.DisablePriority) {
                                if (!comparer.ContainsSpecialRunOfLabels(state.Labels)) {
                                    state.Paused = true;
                                    break;
                                }
                            }
                        }
                    }
                }

                #endregion

                #region 处理空通道相关

                // 如果该通道是空的，看是否有默认状态可以补充该通道
                if (list.Count == 0) {
                    if (this.Defaults != null) {
                        if (this.Defaults.TryGetValue(aisle, out var defaultFsmStateIntricate)) {
                            defaultFsmStateIntricate.Enter(this);
                        }
                    }
                }

                #endregion
            }
        }

        protected void Add(IPriorityState state) {
            this.States.Add(state);

            var aisles = state.Aisles;
            for (int i = 0, len = aisles.Count; i < len; i++) {
                var aisle = aisles[i];
                if (this.Aisles.TryGetValue(aisle, out var list)) {
                    list.Add(state);
                    // 进行排序，把 nowPriority 大的放在前面
                    // states.Sort(); // 不需要排序，因为状态在运行的过程中，nowPriority可能会变的，排序也没大用
                }
                else {
                    this.Aisles.Add(aisle, state);
                }

                if (!this.DirtyFlags.Contains(aisle)) {
                    this.DirtyFlags.Enqueue(aisle);
                }
            }

            state.Manager = this;
            this.CurrentEnterState = state;
            this.EvaluateSuccessdCache = null;
        }

        protected void Remove(IPriorityState state, PriorityStateLeaveType leaveType, IPriorityState initiator) {
            if (!this.States.Remove(state)) return;

            var aisles = state.Aisles;
            for (int i = 0, len = aisles.Count; i < len; i++) {
                var aisle = aisles[i];
                if (this.Aisles.TryGetValue(aisle, out var list)) {
                    if (!list.Remove(state)) {
                        Log.Error($"<从通道中移除状态出现异常，通道不包含该状态> 异常状态:'{state.Name}'，通道名称：'{aisle}'");
                    }
                }
                else {
                    Log.Error($"<从通道中移除状态出现异常，不包含该通道> 异常状态：'{state.Name}'，通道名称：'{aisle}'");
                }

                if (!this.DirtyFlags.Contains(aisle)) {
                    this.DirtyFlags.Enqueue(aisle);
                }
            }

            state.Manager = null;
            if (this.CurrentEnterState == state) {
                this.CurrentEnterState = null;
            }

            state.InternalOnLeave(this);
            state.InternalOnLeaveDetails(this, new PriorityStateLeaveDetails { leaveType = leaveType, initiator = initiator });
            try {
                this.OnStateChanged(state, false);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            this.EvaluateSuccessdCache = null;
        }

        /// <summary>
        /// 检查进入
        /// </summary>
        /// <param name="state"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        protected bool CheckEnter(IPriorityState state, IPriorityStateEnterFailDetails details) {
            // 遍历所有当前状态想要进入的通道，并和通道里的所有状态一一对比，全部通过，才判定为可以进入。
            var aisles = state.Aisles;
            var multiAisle = aisles.Count > 1;
            if (multiAisle) this.ResetHandledTokens();
            // 遍历要进入的状态的通道
            for (int i = 0, len = aisles.Count; i < len; i++) {
                var aisle = aisles[i];
                // 如果该状态机里没有该通道，则可以认为该状态是该通道的第一个，那必然可进入
                if (!this.Aisles.TryGetValue(aisle, out var states)) {
                    continue;
                }

                // 遍历该通道里所有的状态
                foreach (var other in states) {
                    // 如果该状态已经存在于临时区，则继续
                    if (multiAisle && other.HandledFlag) continue;

                    // 获得这个对比者，用它和要进入的状态做对比
                    if (!this.ContrastEnter(state, other, details)) {
                        return false;
                    }

                    // 把对比者加入缓存区，之后便不需要再次和它对比
                    if (multiAisle) other.HandledFlag = true;
                }
            }

            return true;
        }

        /// <summary>
        /// 对比状态进入相关
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contrast"></param>
        /// <param name="details"></param>
        /// <returns>true 代表 source 相对于 contrast 可以进入</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ContrastEnter(IPriorityState source, IPriorityState contrast, IPriorityStateEnterFailDetails details) {
            // 进入优先级 > 对比者
            if (source.EnterPriority > contrast.ResistPriority) {
                // 如果被对比者点名拦截或者点名标签拦截，则依然返回 false
                var result = !contrast.ContainsSpecialInterceptOfLabels(source.Labels);
                if (!result) {
                    if (details != null) {
                        details.FailType = PriorityStateEnterFailType.SpecialIntercept;
                        details.Blocker = contrast;
                    }
                }

                return result;
            }
            else {
                // 如果被对比者点名通过或者点名标签通过，则依然返回 true
                var result = contrast.ContainsSpecialPassedOfLabels(source.Labels);
                if (!result) {
                    if (details != null) {
                        details.FailType = PriorityStateEnterFailType.PriorityLow;
                        details.Blocker = contrast;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 检查排挤
        /// </summary>
        /// <param name="state"></param>
        protected void CheckExclusion(IPriorityState state) {
            // 遍历对比者要对比的所有通道，和每个通道里的所有状态做对比，把能排挤掉的都排挤掉
            var aisles = state.Aisles;
            var multiAisle = aisles.Count > 1;
            this.ExclusionsCache.Clear();
            if (multiAisle) this.ResetHandledTokens();
            // 遍历对比者要求的所有通道
            for (int i = 0, len = aisles.Count; i < len; i++) {
                var aisle = aisles[i];
                if (!this.Aisles.TryGetValue(aisle, out var states)) {
                    // 没有对应的通道很正常，状态机创建一开始，通道是空的，会根据进入的状态来拓展通道
                    continue;
                }

                // 遍历通道里的每个状态
                foreach (var other in states) {
                    // 如果该状态已经被排挤过了
                    if (multiAisle && other.HandledFlag) continue;

                    if (this.ContrastExclusion(other, state)) {
                        this.ExclusionsCache.Add(other);
                    }

                    if (multiAisle) other.HandledFlag = true;
                }
            }

            for (int i = 0, len = this.ExclusionsCache.Count; i < len; i++) {
                this.Remove(this.ExclusionsCache[i], PriorityStateLeaveType.Exclusion, state);
            }
        }

        /// <summary>
        /// 对比状态排挤相关
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contrast"></param>
        /// <returns>true 代表 source 会被 contrast 排挤掉</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool ContrastExclusion(IPriorityState source, IPriorityState contrast) {
            // 自己不会排挤自己
            if (source == contrast) {
                return false;
            }

            // 自己的保持优先级 >= 对比者的排挤优先级, 代表 source 不会被排挤掉
            if (source.KeepPriority >= contrast.ExclusionPriority) {
                // 如果自己被对比者点名或标签排挤，则依然返回 true
                return contrast.ContainsSpecialExclusionOfLabels(source.Labels);
            }
            else {
                // 如果自己被对比者点名或标签保持，则依然返回 false
                return !contrast.ContainsSpecialKeepOfLabels(source.Labels);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void ResetHandledTokens() {
            foreach (var state in this.States) {
                state.HandledFlag = false;
            }
        }

        protected void OnStateChanged(IPriorityState state, bool isEnter) { }

        public bool Evaluate(IPriorityState state, bool successCache, IPriorityStateEnterFailDetails details) {
            if (state.Manager != null) {
                if (state.Manager != this) {
                    throw new Exception($"<一个状态同时只能进入一个状态机> '{state.Name}'");
                }
            }

            var @case = this.States.Contains(state) ? 2 : 0;
            switch (@case) {
                // 如果是完全相同一个状态要再次进入视为重进
                case 2:
                    // 不需要再次和通道内的对比，直接和自己对比，条件满足，则进入成功，并调用进入事件
                    if (state.EnterPriority > state.ResistPriority) {
                        goto SUCCESS;
                    }

                    if (details != null) {
                        details.FailType = PriorityStateEnterFailType.PriorityLow;
                        details.Blocker = state;
                    }

                    break;

                // // 同名时，视为替换。除了必然会移除之前的状态外，其他的和进入一个全新的状态没有差别
                // case 1:
                //     if (this.CheckEnter(state, details)) {
                //         return true;
                //     }
                //
                //     break;

                // 全新的状态，走正常流程
                case 0:
                    // 先检测是否够条件进入
                    if (this.CheckEnter(state, details)) {
                        goto SUCCESS;
                    }

                    break;
            }

            return false;

            SUCCESS:
            if (successCache) this.EvaluateSuccessdCache = state;
            return true;
        }

        public bool Enter(IPriorityState state, IPriorityStateEnterFailDetails details) {
            if (state.Manager != null) {
                if (state.Manager != this) {
                    throw new Exception($"<一个状态同时只能进入一个状态机> '{state.Name}'");
                }
            }

            var @case = this.States.Contains(state) ? 2 : 0;
            switch (@case) {
                // 如果是完全相同一个状态要再次进入视为重进
                case 2:
                    // 不需要再次和通道内的对比，直接和自己对比，条件满足，则进入成功，并调用进入事件
                    if (state.EnterPriority > state.ResistPriority) {
                        // 做做样子，tree本地并没有做任何修改
                        state.InternalOnLeave(this);
                        state.InternalOnLeaveDetails(this, new PriorityStateLeaveDetails { leaveType = PriorityStateLeaveType.ReEnter });
                        state.InternalOnEnter(this);
                        return true;
                    }

                    if (details != null) {
                        details.FailType = PriorityStateEnterFailType.PriorityLow;
                        details.Blocker = state;
                    }

                    break;

                // // 同名时，视为替换。除了必然会移除之前的状态外，其他的和进入一个全新的状态没有差别
                // case 1:
                //     if (this.CheckEnter(state, details)) {
                //         this.Remove(subrogee, PriorityStateLeaveType.Replace, state);
                //         this.Add(state);
                //         this.CheckExclusion(state);
                //         state.InternalOnEnter(this);
                //         if (!state.IsEntered) {
                //             // 自己排挤别的人时候，别人在Leave回调中又导致进入了别的状态，排挤掉了自己，这种情况，就不往下执行了
                //             return true;
                //         }
                //
                //         try {
                //             this.OnStateChanged(state, true);
                //         }
                //         catch (Exception e) {
                //             Log.Error(e);
                //         }
                //
                //         return true;
                //     }
                //
                //     break;

                // 全新的状态，走正常流程
                case 0:
                    // 先检测是否够条件进入
                    if (state == this.EvaluateSuccessdCache || this.CheckEnter(state, details)) {
                        // 先把状态加入
                        this.Add(state);
                        // 然后把它能排挤掉的状态排挤掉
                        this.CheckExclusion(state);
                        // 禁掉能禁用的其他状态
                        state.InternalOnEnter(this);
                        if (!state.IsEntered) {
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

                    break;
            }

            return false;
        }

        public bool Leave(IPriorityState state) {
            if (state == null) {
                Log.Error("状态为空!");
                return false;
            }

            if (!this.States.Contains(state)) {
                return false;
            }

            this.Remove(state, PriorityStateLeaveType.InitiativeInvoke, null);
            return true;
        }

        // 外部调用, 以正常运行
        public void Update(float deltaTime) {
            if (this.DirtyFlags.Count > 0) {
                this.HandleDirtiness();
            }

            // timeScale 不允许为小于0，不然倒着跑了
            if (this.TimeScale < 0) {
                this.TimeScale = 0;
            }

            var finalDeltaTime = deltaTime * this.TimeScale;

            this.PollingCache.Clear();
            foreach (var state in this.States) {
                this.PollingCache.Push(state);
            }

            while (this.PollingCache.Count != 0) {
                var state = this.PollingCache.Pop();
                if (!this.States.Contains(state)) {
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

        public bool Contains(string name) {
            foreach (var state in this.States) {
                if (state.Name == name)
                    return true;
            }

            return false;
        }

        public IPriorityState Get(string name) {
            foreach (var state in this.States) {
                if (state.Name == name)
                    return state;
            }

            return null;
        }

        /// <summary>
        /// 设置一个默认的行为，当该通道没有行为的时候，会使用该默认行为，一般使用在主通道里
        /// </summary>
        /// <param name="aisle">一般使用0代表主通道</param>
        /// <param name="state"></param>
        public void SetDefaultState(int aisle, IPriorityState state) {
            if (this.Defaults == null) return;
            this.Defaults[aisle] = state;
            if (!this.Aisles.TryGetValue(aisle, out var list)) {
                state.Enter(this);
                return;
            }

            if (list.Count == 0) {
                state.Enter(this);
            }
        }

        public void SetDefaultState(IPriorityState state) {
            foreach (var aisle in state.Aisles) {
                this.SetDefaultState(aisle, state);
            }
        }

        public IPriorityState GetDefaultState(int aisle) {
            if (this.Defaults == null) return null;
            if (this.Defaults.TryGetValue(aisle, out var result)) {
                return result;
            }

            return null;
        }

        public void RemoveDefaultState(int aisle, IPriorityState state) {
            if (this.Defaults == null) return;
            if (!this.Defaults.TryGetValue(aisle, out var result)) return;
            if (result != state) return;
            this.Defaults.Remove(aisle);
        }

        public void RemoveDefaultState(IPriorityState state) {
            foreach (var aisle in state.Aisles) {
                this.RemoveDefaultState(aisle, state);
            }
        }
    }
}