using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Hsenl {
    // internal 的虚函数, 不用管
    // internal 的抽象函数, 在继承类里实现, 用于回调给接口一些需要用到的数据
    // protected 是输出给继承类的接口, 需要就隐式实现, 如果还需要继续继承, 就自行额外实现一个虚函数
    // public 的虚函数, 也可以不用管, 继承类直接强转调用, 有需要也可以实现自己的逻辑, 以覆盖接口内的逻辑
    // protected 和 public 作为都是暴露给外部程序集的接口, 是优先级系统设计中, 必须存在的东西
    public interface IPriorityState {
        public bool HandledFlag { get; set; }
        public float TimeScale { get; set; }
        public string Name { get; set; }
        public IList<int> Aisles { get; set; }
        public Bitlist Labels { get; set; }

        // 举例, 如果两个状态, 六个参数都为默认值, A: 0 0 0 0 0 0, B: 0 0 0 0 0 0, 那么结果就是 无法进入、不会被排挤、不会被禁用
        // 参考:
        // 技能: 排挤等级设置 > 0, 因为技能一般来说, 不会同时释放两个技能
        // 状态: 排挤保持为0就好, 因为一般来说, 状态是可以共存的
        public int EnterPriority { get; set; } // 该等级必须 > 对方的阻碍等级才可以进入
        public int ResistPriorityAnchor { get; set; } // 当状态重新进入时, ResistPriority会重置为该值
        public int ResistPriority { get; set; } // 该等级只要 >= 对方的进入等级就可以阻止对方进入
        public int ExclusionPriority { get; set; } // 该等级必须 > 对方的保持等级才可以排挤掉对方
        public int KeepPriority { get; set; } // 该等级 >= 对方的排挤等级才可以保持住自己
        public int DisablePriority { get; set; } // 该等级必须 > 对方的运行等级才可以禁用掉对方
        public int RunPriority { get; set; } // 该等级 >= 对方的禁用等级才可以保证自己继续运行

        public Bitlist SpecialPassLabels { get; set; }
        public Bitlist SpecialInterceptLabels { get; set; }
        public Bitlist SpecialExclusionLabels { get; set; }
        public Bitlist SpecialKeepLabels { get; set; }
        public Bitlist SpecialDisableLabels { get; set; }
        public Bitlist SpecialRunLabels { get; set; }

        protected bool PausedPrevious { get; set; }
        public bool Paused { get; set; } // 暂停后，OnRunning将不再运行
        public float Duration { get; set; } // 持续时间
        public float Time { get; set; } // 当前时间
        public bool TimeParse { get; set; } // 暂停后，唯一产生的影响是，time不再发生变化
        public IPrioritizer Manager { get; set; } // 该值, 只由manager进行修改

        public bool IsEntered => this.Manager != null;

        internal void InternalOnEnter(IPrioritizer manager) {
            this.ResetState();
            try {
                this.OnEnter(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为进入错误> {this.Name} --- {e}");
            }
        }

        internal void InternalOnRefresh(IPrioritizer manager) {
            this.Time = 0;
            try {
                this.OnRefresh(manager);
            }
            catch (Exception e) {
                Log.Error($"<刷新行为错误> {this.Name} --- {e}");
            }
        }

        internal void InternalOnEnable(IPrioritizer manager) {
            try {
                this.OnEnable(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为开启错误> {this.Name} --- {e}");
            }
        }

        internal void InternalOnUpdate(IPrioritizer manager, float deltaTime) {
            try {
                this.OnUpdate(manager, deltaTime);
            }
            catch (Exception e) {
                Log.Error($"<行为运行错误> {this.Name} --- {e}");
            }
        }

        internal void InternalOnDisable(IPrioritizer manager) {
            try {
                this.OnDisable(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为禁用错误> {this.Name} --- {e}");
            }
        }

        internal void InternalOnLeave(IPrioritizer manager) {
            try {
                this.OnLeave(manager);
            }
            catch (Exception e) {
                Log.Error($"<行为离开错误> {this.Name} --- {e}");
            }
            finally {
                this.ResetState();
            }
        }

        internal void InternalOnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails) {
            try {
                this.OnLeaveDetails(manager, leaveDetails);
            }
            catch (Exception e) {
                Log.Error($"<行为离开错误(详细)> {this.Name} --- {e}");
            }
            finally {
                this.ResetState();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialPassedOfLabels(Bitlist lbs) => this.SpecialPassLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialInterceptOfLabels(Bitlist lbs) => this.SpecialInterceptLabels?.ContainsAny(lbs) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialExclusionOfLabels(Bitlist lbs) => this.SpecialExclusionLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialKeepOfLabels(Bitlist lbs) => this.SpecialKeepLabels?.ContainsAny(lbs) ?? false;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialDisableOfLabels(Bitlist lbs) => this.SpecialDisableLabels?.ContainsAny(lbs) ?? false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ContainsSpecialRunOfLabels(Bitlist lbs) => this.SpecialRunLabels?.ContainsAny(lbs) ?? false;

        public void AddSpecialPassedOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialPassLabels ??= new Bitlist();
            this.SpecialPassLabels.Add(lbs);
        }

        public void AddSpecialInterceptOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialInterceptLabels ??= new Bitlist();
            this.SpecialInterceptLabels.Add(lbs);
        }

        public void AddSpecialExclusionOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialExclusionLabels ??= new Bitlist();
            this.SpecialExclusionLabels.Add(lbs);
        }

        public void AddSpecialKeepOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialKeepLabels ??= new Bitlist();
            this.SpecialKeepLabels.Add(lbs);
        }

        public void AddSpecialDisableOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialDisableLabels ??= new Bitlist();
            this.SpecialDisableLabels.Add(lbs);
        }

        public void AddSpecialRunOfLabels(List<int> lbs) {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialRunLabels ??= new Bitlist();
            this.SpecialRunLabels.Add(lbs);
        }

        public void AddSpecialPassedOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialPassLabels ??= new Bitlist();
            this.SpecialPassLabels.Add(lbs);
        }

        public void AddSpecialInterceptOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialInterceptLabels ??= new Bitlist();
            this.SpecialInterceptLabels.Add(lbs);
        }

        public void AddSpecialExclusionOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialExclusionLabels ??= new Bitlist();
            this.SpecialExclusionLabels.Add(lbs);
        }

        public void AddSpecialKeepOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialKeepLabels ??= new Bitlist();
            this.SpecialKeepLabels.Add(lbs);
        }

        public void AddSpecialDisableOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialDisableLabels ??= new Bitlist();
            this.SpecialDisableLabels.Add(lbs);
        }

        public void AddSpecialRunOfLabels<T>(List<T> lbs) where T : Enum {
            if (lbs == null || lbs.Count == 0) return;
            this.SpecialRunLabels ??= new Bitlist();
            this.SpecialRunLabels.Add(lbs);
        }

        public void ResetState() {
            this.Paused = false;
            this.PausedPrevious = true; // 让行为初次进入时, 也能触发一次OnEnable
            this.ResistPriority = this.ResistPriorityAnchor;
            this.Time = 0;
            this.TimeParse = false;
        }

        public byte Run(float deltaTime) {
            // 暂停 - 上次如果没暂停，则调佣 onDisable
            if (this.Paused) {
                if (!this.PausedPrevious) {
                    this.OnDisable(this.Manager);
                }

                this.PausedPrevious = this.Paused;
                return 0;
            }

            // 如果上次是暂停状态，则调用 onEnable
            if (this.PausedPrevious) {
                this.OnEnable(this.Manager);
            }

            var dt = deltaTime * this.TimeScale;
            // Update
            this.OnUpdate(this.Manager, dt);

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

            this.PausedPrevious = this.Paused;

            return 0;
        }

        protected void OnEnter(IPrioritizer manager);
        protected void OnRefresh(IPrioritizer manager);
        protected void OnEnable(IPrioritizer manager);
        protected void OnUpdate(IPrioritizer manager, float deltaTime);
        protected void OnDisable(IPrioritizer manager);
        protected void OnLeave(IPrioritizer manager);

        /// <param name="manager"></param>
        /// <param name="leaveDetails"></param>
        protected void OnLeaveDetails(IPrioritizer manager, PriorityStateLeaveDetails leaveDetails);

        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool Enter(IPrioritizer manager, IPriorityStateEnterFailDetails details = null) {
            return manager.Enter(this, details);
        }

        /// <summary>
        /// 评估是否能进入
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="successCache">如果成功, 则缓存该状态, 在优先器内的任何状态发生改变之前, 该状态进入时, 不需要再次对比检查,
        /// 使用前要自行确定这期间, 不会有影响公正性的改变. 因为优先器只监视添加和删除状态导致的改变, 而监视不了状态自己本身发生的改变, 比如修改了自身的优先级</param>
        /// <param name="details"></param>
        /// <returns></returns>
        public bool Evaluate(IPrioritizer manager, bool successCache = false, IPriorityStateEnterFailDetails details = null) {
            return manager.Evaluate(this, successCache, details);
        }

        /// <summary>
        /// 刷新, 会重置时间, 并且触发一次刷新回调
        /// </summary>
        /// <returns></returns>
        public bool Refresh() {
            if (this.IsEntered == false) {
                return false;
            }

            this.OnRefresh(this.Manager);
            return true;
        }

        /// <summary>
        /// 离开状态机
        /// </summary>
        /// <returns></returns>
        public bool Leave() {
            return this.IsEntered && this.Manager.Leave(this);
        }
    }
}