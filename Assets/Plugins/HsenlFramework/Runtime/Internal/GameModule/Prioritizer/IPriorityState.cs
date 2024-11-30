using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Hsenl {
    // internal 的虚函数, 不用管
    // internal 的抽象函数, 在继承类里实现, 用于回调给接口一些需要用到的数据
    // protected 是输出给继承类的接口, 需要就隐式实现, 如果还需要继续继承, 就自行额外实现一个虚函数
    // public 的虚函数, 也可以不用管, 继承类直接强转调用, 有需要也可以实现自己的逻辑, 以覆盖接口内的逻辑
    // protected 和 public 作为都是暴露给外部程序集的接口, 是优先级系统设计中, 必须存在的东西
    public interface IPriorityState {
        protected internal bool handledFlag { get; set; }
        public float TimeScale { get; }
        public string Name { get; }
        protected internal IList<int> aisles { get; set; }
        protected internal Bitlist labels { get; }

        // 举例, 如果两个状态, 六个参数都为默认值, A: 0 0 0 0 0 0, B: 0 0 0 0 0 0, 那么结果就是 无法进入、不会被排挤、不会被禁用
        // 参考:
        // 技能: 排挤等级设置 > 0, 因为技能一般来说, 不会同时释放两个技能
        // 状态: 排挤保持为0就好, 因为一般来说, 状态是可以共存的
        public int EnterPriorityAnchor { get; protected set; } // 当状态离开时, EnterPriority会重置为该值
        public int EnterPriority { get; protected set; } // 该等级必须 >= 对方的阻碍等级才可以进入
        public int ObstructPriorityAnchor { get; protected set; }
        public int ObstructPriority { get; protected set; } // 该等级只要 >= 对方的进入等级就可以阻止对方进入
        public int KeepPriorityAnchor { get; protected set; }
        public int KeepPriority { get; protected set; } // 该等级 >= 对方的排挤等级才可以保持住自己
        public int ExclusionPriorityAnchor { get; protected set; }
        public int ExclusionPriority { get; protected set; } // 该等级必须 > 对方的保持等级才可以排挤掉对方
        public int RunPriorityAnchor { get; protected set; }
        public int RunPriority { get; protected set; } // 该等级 >= 对方的禁用等级才可以保证自己继续运行
        public int DisablePriorityAnchor { get; protected set; }
        public int DisablePriority { get; protected set; } // 该等级必须 > 对方的运行等级才可以禁用掉对方

        protected Bitlist _specialPassLabels { get; set; }
        protected internal Bitlist specialObstructLabels { get; set; }
        protected Bitlist _specialKeepLabels { get; set; }
        protected Bitlist _specialExclusionLabels { get; set; }
        protected Bitlist _specialRunLabels { get; set; }
        protected internal Bitlist specialDisableLabels { get; set; }

        protected bool _pausedPrevious { get; set; }
        public bool Paused { get; protected internal set; } // 暂停后，OnRunning将不再运行
        public float Duration { get; } // 持续时间
        public float Time { get; set; } // 当前时间
        public bool TimeParse { get; set; } // 暂停后，唯一产生的影响是，time不再发生变化
        public IPrioritizer Manager { get; protected internal set; } // 该值, 只由manager进行修改

        public bool IsEntered_Interface => this.Manager != null;

        internal void OnEnter_Internal(IPrioritizer manager) {
            this.ResetState();
            try {
                this.OnEnter(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为进入错误> {this.Name} --- {e}");
            }
        }

        internal void OnRefresh_Internal(IPrioritizer manager) {
            this.Time = 0;
            try {
                this.OnRefresh(manager);
            }
            catch (Exception e) {
                Log.Error($"<刷新行为错误> {this.Name} --- {e}");
            }
        }

        internal void OnEnable_Internal(IPrioritizer manager) {
            try {
                this.OnEnable(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为开启错误> {this.Name} --- {e}");
            }
        }

        internal void OnUpdate_Internal(IPrioritizer manager, float deltaTime) {
            try {
                this.OnUpdate(manager, deltaTime);
            }
            catch (Exception e) {
                Log.Error($"<行为运行错误> {this.Name} --- {e}");
            }
        }

        internal void OnDisable_Internal(IPrioritizer manager) {
            try {
                this.OnDisable(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为禁用错误> {this.Name} --- {e}");
            }
        }

        internal void OnLeave_Internal(IPrioritizer manager) {
            try {
                this.OnLeave(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为离开错误> {this.Name} --- {e}");
            }
        }

        internal void OnLeaveDetails_Internal(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) {
            try {
                this.OnLeaveDetails(manager, leaveDetails);
            }
            catch (Exception e) {
                Log.Error($"<行为离开错误(详细)> {this.Name} --- {e}");
            }
            finally {
                this.ResetPriority();
            }
        }

        public void InitAisles_Interface(IList<int> aisleIds) {
            if (this.IsEntered_Interface)
                throw new Exception("InitAisles when a entered priorities are not allowed.");

            this.aisles = aisleIds;
        }

        public void InitPriorities_Interface(int enter, int obstruct, int keep, int exclusion, int run, int disable) {
            if (this.IsEntered_Interface)
                throw new Exception("InitPriorities when a entered priorities are not allowed.");

            this.EnterPriorityAnchor = enter;
            this.EnterPriority = enter;
            this.ObstructPriorityAnchor = obstruct;
            this.ObstructPriority = obstruct;
            this.KeepPriorityAnchor = keep;
            this.KeepPriority = keep;
            this.ExclusionPriorityAnchor = exclusion;
            this.ExclusionPriority = exclusion;
            this.RunPriorityAnchor = run;
            this.RunPriority = run;
            this.DisablePriorityAnchor = disable;
            this.DisablePriority = disable;
        }

        public void ModifyEnter_Interface(int enterPriority) {
            this.EnterPriority = enterPriority;
        }

        public void ModifyObstruct_Interface(int obstructPriority) {
            var obs = this.ObstructPriority;
            if (obstructPriority == obs)
                return;

            this.ObstructPriority = obstructPriority;

            if (this.IsEntered_Interface) {
                if (obstructPriority > obs) {
                    this.Manager.RefreshObstructPriority(this, true);
                }
                else {
                    this.Manager.RefreshObstructPriority(this, false);
                }
            }
        }

        public void ModifyKeep_Interface(int keepPriority) {
            var keep = this.KeepPriority;
            if (keepPriority == keep)
                return;

            this.KeepPriority = keep;

            if (this.IsEntered_Interface) {
                if (keepPriority < keep) {
                    this.Manager.RecheckKeep(this);
                }
            }
        }

        public void ModifyExclusion_Interface(int exclusionPriority) {
            var exc = this.ExclusionPriority;
            if (exclusionPriority == exc)
                return;

            this.ExclusionPriority = exclusionPriority;

            if (this.IsEntered_Interface) {
                if (exclusionPriority > exc) {
                    this.Manager.RecheckExclusion(this);
                }
            }
        }

        public void ModifyRun_Interface(int runPriority) {
            var run = this.RunPriority;
            if (runPriority == run)
                return;

            this.RunPriority = runPriority;

            if (this.IsEntered_Interface) {
                this.Manager.RecheckRun(this);
            }
        }

        public void ModifyDisable_Interface(int disablePriority) {
            var dis = this.DisablePriority;
            if (disablePriority == dis)
                return;

            this.DisablePriority = disablePriority;

            if (this.IsEntered_Interface) {
                if (disablePriority > dis) {
                    this.Manager.RefreshDisablePriority(this, true);
                }
                else {
                    this.Manager.RefreshDisablePriority(this, false);
                }
            }
        }

        public bool ContainsAisle(int aisle) {
            for (int i = 0, len = this.aisles.Count; i < len; i++) {
                if (aisle == this.aisles[i]) {
                    return true;
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialPassedOfLabel(int lb) => this._specialPassLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialObstructOfLabel(int lb) => this.specialObstructLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialKeepOfLabel(int lb) => this._specialKeepLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialExclusionOfLabel(int lb) => this._specialExclusionLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialRunOfLabel(int lb) => this._specialRunLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialDisableOfLabel(int lb) => this.specialDisableLabels?.Contains(lb) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialPassedOfLabels(Bitlist lbs) => this._specialPassLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialObstructOfLabels(Bitlist lbs) => this.specialObstructLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialKeepOfLabels(Bitlist lbs) => this._specialKeepLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialExclusionOfLabels(Bitlist lbs) => this._specialExclusionLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialRunOfLabels(Bitlist lbs) => this._specialRunLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialDisableOfLabels(Bitlist lbs) => this.specialDisableLabels?.ContainsAny(lbs) ?? false;

        public void AddSpecialPassedOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this._specialPassLabels ??= new Bitlist();
            if (this._specialPassLabels == null) return;
            this._specialPassLabels.Add(lbs);
        }

        public void AddSpecialObstructOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.specialObstructLabels ??= new Bitlist();
            if (this.specialObstructLabels == null) return;
            this.specialObstructLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshObstructLabels(this, true);
            }
        }

        public void AddSpecialKeepOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this._specialKeepLabels ??= new Bitlist();
            if (this._specialKeepLabels == null) return;
            this._specialKeepLabels.Add(lbs);
        }

        public void AddSpecialExclusionOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this._specialExclusionLabels ??= new Bitlist();
            if (this._specialExclusionLabels == null) return;
            this._specialExclusionLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckExclusion(this);
            }
        }

        public void AddSpecialRunOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this._specialRunLabels ??= new Bitlist();
            if (this._specialRunLabels == null) return;
            this._specialRunLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckRun(this);
            }
        }

        public void AddSpecialDisableOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.specialDisableLabels ??= new Bitlist();
            if (this.specialDisableLabels == null) return;
            this.specialDisableLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshDisableLabels(this, true);
            }
        }

        public void AddSpecialPassedOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this._specialPassLabels ??= new Bitlist();
            if (this._specialPassLabels == null) return;
            this._specialPassLabels.Add(lbs);
        }

        public void AddSpecialObstructOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.specialObstructLabels ??= new Bitlist();
            if (this.specialObstructLabels == null) return;
            this.specialObstructLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshObstructLabels(this, true);
            }
        }

        public void AddSpecialKeepOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this._specialKeepLabels ??= new Bitlist();
            if (this._specialKeepLabels == null) return;
            this._specialKeepLabels.Add(lbs);
        }

        public void AddSpecialExclusionOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this._specialExclusionLabels ??= new Bitlist();
            if (this._specialExclusionLabels == null) return;
            this._specialExclusionLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckExclusion(this);
            }
        }

        public void AddSpecialRunOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this._specialRunLabels ??= new Bitlist();
            if (this._specialRunLabels == null) return;
            this._specialRunLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckRun(this);
            }
        }

        public void AddSpecialDisableOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.specialDisableLabels ??= new Bitlist();
            if (this.specialDisableLabels == null) return;
            this.specialDisableLabels.Add(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshDisableLabels(this, true);
            }
        }

        public void RemoveSpecialPassedOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialPassLabels == null) return;
            this._specialPassLabels.Remove(lbs);
        }

        public void RemoveSpecialObstructOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this.specialObstructLabels == null) return;
            this.specialObstructLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshObstructLabels(this, false);
            }
        }

        public void RemoveSpecialKeepOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialKeepLabels == null) return;
            this._specialKeepLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckKeep(this);
            }
        }

        public void RemoveSpecialExclusionOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialExclusionLabels == null) return;
            this._specialExclusionLabels.Remove(lbs);
        }

        public void RemoveSpecialRunOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialRunLabels == null) return;
            this._specialRunLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckRun(this);
            }
        }

        public void RemoveSpecialDisableOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            if (this.specialDisableLabels == null) return;
            this.specialDisableLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshDisableLabels(this, false);
            }
        }

        public void RemoveSpecialPassedOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialPassLabels == null) return;
            this._specialPassLabels.Remove(lbs);
        }

        public void RemoveSpecialObstructOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this.specialObstructLabels == null) return;
            this.specialObstructLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshObstructLabels(this, false);
            }
        }

        public void RemoveSpecialKeepOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialKeepLabels == null) return;
            this._specialKeepLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckKeep(this);
            }
        }

        public void RemoveSpecialExclusionOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialExclusionLabels == null) return;
            this._specialExclusionLabels.Remove(lbs);
        }

        public void RemoveSpecialRunOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this._specialRunLabels == null) return;
            this._specialRunLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RecheckRun(this);
            }
        }

        public void RemoveSpecialDisableOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            if (this.specialDisableLabels == null) return;
            this.specialDisableLabels.Remove(lbs);

            if (this.IsEntered_Interface) {
                this.Manager.RefreshDisableLabels(this, false);
            }
        }

        private void ResetPriority() {
            this.ModifyEnter_Interface(this.EnterPriorityAnchor);
            this.ModifyObstruct_Interface(this.ObstructPriorityAnchor);
            this.ModifyKeep_Interface(this.KeepPriorityAnchor);
            this.ModifyExclusion_Interface(this.ExclusionPriorityAnchor);
            this.ModifyRun_Interface(this.RunPriorityAnchor);
            this.ModifyDisable_Interface(this.DisablePriorityAnchor);
        }

        private void ResetState() {
            this.Paused = false;
            this._pausedPrevious = true; // 让行为初次进入时, 也能触发一次OnEnable
            this.Time = 0;
            this.TimeParse = false;
        }

        /// <param name="manager"></param>
        /// <param name="leaveDetails"></param>
        protected void OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails);

        /// <summary>
        /// 评估是否能进入
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="successCache">如果成功, 则缓存该状态, 在优先器内的任何状态发生改变之前, 该状态进入时, 不需要再次对比检查,
        /// 使用前要自行确定这期间, 不会有影响公正性的改变. 因为优先器只监视添加和删除状态导致的改变, 而监视不了状态自己本身发生的改变, 比如修改了自身的优先级</param>
        /// <returns></returns>
        public bool Evaluate_Interface(IPrioritizer manager, bool successCache = false) {
            return manager.Evaluate(this, successCache, out _);
        }

        public bool Evaluate_Interface(IPrioritizer manager, out PriorityStateEnterDetails details, bool successCache = false) {
            return manager.Evaluate(this, successCache, out details);
        }

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool Enter_Interface(IPrioritizer manager) {
            return manager.Enter(this, out _);
        }

        public bool Enter_Interface(IPrioritizer manager, out PriorityStateEnterDetails details) {
            return manager.Enter(this, out details);
        }

        /// <summary>
        /// 刷新, 会重置时间, 并且触发一次刷新回调
        /// </summary>
        /// <returns></returns>
        public bool Refresh_Interface() {
            if (!this.IsEntered_Interface) {
                return false;
            }

            this.OnRefresh_Internal(this.Manager);
            return true;
        }

        /// <summary>
        /// 离开状态机
        /// </summary>
        /// <returns></returns>
        public bool Leave_Interface() {
            return this.IsEntered_Interface && this.Manager.Leave(this);
        }

        internal byte Run(float deltaTime) {
            // 暂停 - 上次如果没暂停，则调佣 onDisable
            if (this.Paused) {
                if (!this._pausedPrevious) {
                    this.OnDisable_Internal(this.Manager);
                }

                this._pausedPrevious = this.Paused;
                return 0;
            }

            // 如果上次是暂停状态，则调用 onEnable
            if (this._pausedPrevious) {
                this.OnEnable_Internal(this.Manager);
            }

            var dt = deltaTime * this.TimeScale;
            // Update
            this.OnUpdate_Internal(this.Manager, dt);

            // 跑时间，即使y<0，也不会影响计时，只有时间暂停才会影响计时，且只会影响计时
            if (!this.TimeParse) {
                this.Time += dt;
            }

            // 判断计时是否到了
            if (this.Time >= this.Duration) {
                if (this.Duration >= 0) {
                    return 1; // 超时
                }
                else {
                    return 0; // y小于0，代表不受时间限制
                }
            }

            this._pausedPrevious = this.Paused;

            return 0;
        }

        protected void OnEnter(IPrioritizer manager);
        protected void OnRefresh(IPrioritizer manager);
        protected void OnEnable(IPrioritizer manager);
        protected void OnUpdate(IPrioritizer manager, float deltaTime);
        protected void OnDisable(IPrioritizer manager);
        protected void OnLeave(IPrioritizer manager);
    }
}