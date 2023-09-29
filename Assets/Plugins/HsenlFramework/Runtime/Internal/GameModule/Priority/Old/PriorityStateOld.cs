// using System;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
//
// namespace ET {
//     public abstract class PriorityState : Component {
//         internal abstract bool InternalHandledFlag { get; set; }
//
//         internal abstract float InternalTimeScale { get; }
//         internal abstract string InternalName { get; } // 在manager里面期间, 不允许修改
//         internal abstract ICollection<int> InternalAisles { get; } // 在manager里面期间, 不允许修改
//         internal abstract BitList InternalLabels { get; }
//
//         // 举例, 如果两个状态, 六个参数都为默认值, A: 0 0 0 0 0 0, B: 0 0 0 0 0 0, 那么结果就是 无法进入、不会被排挤、不会被禁用
//         // 参考:
//         // 技能: 排挤等级设置 > 0, 因为技能一般来说, 不会同时释放两个技能
//         // 状态: 排挤保持为0就好, 因为一般来说, 状态是可以共存的
//         internal abstract int InternalEnterPriority { get; } // 该等级必须 > 对方的阻碍等级才可以进入
//         internal abstract int InternalResistPriorityAnchor { get; } // 当状态重新进入时, ResistPriority会重置为该值
//         internal abstract int InternalResistPriority { get; set; } // 该等级只要 >= 对方的进入等级就可以阻止对方进入
//         internal abstract int InternalKeepPriority { get; } // 该等级 >= 对方的排挤等级才可以保持住自己
//         internal abstract int InternalExclusionPriority { get; } // 该等级必须 > 对方的保持等级才可以排挤掉对方
//         internal abstract int InternalRunPriority { get; } // 该等级 >= 对方的禁用等级才可以保证自己继续运行
//         internal abstract int InternalDisablePriority { get; } // 该等级必须 > 对方的运行等级才可以禁用掉对方
//
//         internal abstract BitList InternalSpecialPassLabels { get; }
//         internal abstract BitList InternalSpecialInterceptLabels { get; }
//         internal abstract BitList InternalSpecialKeepLabels { get; }
//         internal abstract BitList InternalSpecialExclusionLabels { get; }
//         internal abstract BitList InternalSpecialRunLabels { get; }
//         internal abstract BitList InternalSpecialDisableLabels { get; }
//
//         internal abstract bool InternalPausedPrevious { get; set; }
//         internal abstract bool InternalPaused { get; set; } // 暂停后，OnRunning将不再运行
//         internal abstract float InternalDuration { get; set; } // 持续时间
//         internal abstract float InternalDurationTimer { get; set; } // 持续时间计时器
//         internal abstract bool InternalTimeParse { get; set; } // 暂停后，唯一产生的影响是，time不再发生变化
//         internal abstract Priorities InternalManager { get; set; } // 该值, 只由manager进行修改
//
//         internal bool InternalIsEntered => this.InternalManager != null;
//
//         internal void InternalOnEnter(Priorities manager) {
//             this.InternalReset();
//             try {
//                 this.OnEnter(manager);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为进入错误> {this.InternalName} --- {e}");
//             }
//         }
//
//         internal void InternalOnRefresh(Priorities manager) {
//             this.InternalDurationTimer = 0;
//             try {
//                 this.OnRefresh(manager);
//             }
//             catch (Exception e) {
//                 Log.Error($"<刷新行为错误> {this.InternalName} --- {e}");
//             }
//         }
//
//         internal void InternalOnEnable(Priorities manager) {
//             try {
//                 this.OnEnable(manager);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为开启错误> {this.InternalName} --- {e}");
//             }
//         }
//
//         internal void InternalOnUpdate(Priorities manager, float deltaTime) {
//             try {
//                 this.OnUpdate(manager, deltaTime);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为运行错误> {this.InternalName} --- {e}");
//             }
//         }
//
//         internal void InternalOnDisable(Priorities manager) {
//             try {
//                 this.OnDisable(manager);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为禁用错误> {this.InternalName} --- {e}");
//             }
//         }
//
//         internal void InternalOnLeave(Priorities manager) {
//             try {
//                 this.OnLeave(manager);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为离开错误> {this.InternalName} --- {e}");
//             }
//             finally {
//                 this.InternalReset();
//             }
//         }
//
//         internal void InternalOnLeaveDetails(Priorities manager, PriorityStateLeaveType leaveType, PriorityState initiator) {
//             try {
//                 this.OnLeaveDetails(manager, leaveType, initiator);
//             }
//             catch (Exception e) {
//                 Log.Error($"<行为离开错误(详细)> {this.InternalName} --- {e}");
//             }
//             finally {
//                 this.InternalReset();
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialPassedOfLabels(BitList lbs) => this.InternalSpecialPassLabels?.ContainsAny(lbs) ?? false;
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialInterceptOfLabels(BitList lbs) => this.InternalSpecialInterceptLabels?.ContainsAny(lbs) ?? false;
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialKeepOfLabels(BitList lbs) => this.InternalSpecialKeepLabels?.ContainsAny(lbs) ?? false;
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialExclusionOfLabels(BitList lbs) => this.InternalSpecialExclusionLabels?.ContainsAny(lbs) ?? false;
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialRunOfLabels(BitList lbs) => this.InternalSpecialRunLabels?.ContainsAny(lbs) ?? false;
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal bool InternalContainsSpecialDisableOfLabels(BitList lbs) => this.InternalSpecialDisableLabels?.ContainsAny(lbs) ?? false;
//         
//         internal void InternalReset() {
//             this.InternalPaused = false;
//             this.InternalPausedPrevious = true; // 让行为初次进入时, 也能触发一次OnEnable
//             this.InternalResistPriority = this.InternalResistPriorityAnchor;
//             this.InternalDurationTimer = 0;
//             this.InternalTimeParse = false;
//         }
//
//         internal byte InternalRun(float deltaTime) {
//             // 暂停 - 上次如果没暂停，则调佣 onDisable
//             if (this.InternalPaused) {
//                 if (!this.InternalPausedPrevious) {
//                     this.InternalOnDisable(this.InternalManager);
//                 }
//
//                 this.InternalPausedPrevious = this.InternalPaused;
//                 return 0;
//             }
//
//             // 如果上次是暂停状态，则调用 onEnable
//             if (this.InternalPausedPrevious) {
//                 this.InternalOnEnable(this.InternalManager);
//             }
//
//             var dt = deltaTime * this.InternalTimeScale;
//             // Update
//             this.InternalOnUpdate(this.InternalManager, dt);
//
//             // 跑时间，即使y<0，也不会影响计时，只有时间暂停才会影响计时，且只会影响计时
//             if (!this.InternalTimeParse) {
//                 this.InternalDurationTimer += dt;
//             }
//
//             // 判断计时是否到了
//             if (this.InternalDurationTimer >= this.InternalDuration) {
//                 if (this.InternalDuration >= 0) {
//                     return 1; // 超时
//                 }
//                 else {
//                     return 0; // y小于0，代表不受时间限制
//                 }
//             }
//
//             this.InternalPausedPrevious = this.InternalPaused;
//
//             return 0;
//         }
//         
//         protected virtual void OnEnter(Priorities manager) { }
//         protected virtual void OnRefresh(Priorities manager) { }
//         protected virtual void OnEnable(Priorities manager) { }
//         protected virtual void OnUpdate(Priorities manager, float deltaTime) { }
//         protected virtual void OnDisable(Priorities manager) { }
//         protected virtual void OnLeave(Priorities manager) { }
//
//         /// <param name="manager"></param>
//         /// <param name="leaveType">离开的原因</param>
//         /// <param name="initiator">导致自己离开的始作俑者</param>
//         protected void OnLeaveDetails(Priorities manager, PriorityStateLeaveType leaveType, PriorityState initiator) { }
//
//         /// <summary>
//         /// 进入
//         /// </summary>
//         /// <param name="manager"></param>
//         /// <param name="details"></param>
//         /// <returns></returns>
//         public bool Enter(Priorities manager, PriorityStateEnterFailDetails details = null) {
//             return manager.InternalEnter(this, details);
//         }
//
//         /// <summary>
//         /// 测试是否能进入，但不真的进入
//         /// </summary>
//         /// <param name="manager"></param>
//         /// <param name="details"></param>
//         /// <returns></returns>
//         public bool RehearsalEnter(Priorities manager, PriorityStateEnterFailDetails details = null) {
//             return manager.InternalRehearsalEnter(this, details);
//         }
//
//         /// <summary>
//         /// 刷新, 会重置时间, 并且触发一次刷新回调
//         /// </summary>
//         /// <returns></returns>
//         public bool Refresh() {
//             if (this.InternalIsEntered == false) {
//                 return false;
//             }
//
//             this.InternalOnRefresh(this.InternalManager);
//             return true;
//         }
//
//         /// <summary>
//         /// 离开状态机
//         /// </summary>
//         /// <returns></returns>
//         public bool Leave() {
//             return this.InternalIsEntered && this.InternalManager.InternalLeave(this);
//         }
//
//         #region 丢弃的代码
//
//         // public virtual void AddSpecialPassedLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialPassLabels ??= new BitList();
//         //     this.SpecialPassLabels?.Add(list);
//         // }
//         //
//         // public virtual void AddSpecialInterceptLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialInterceptLabels ??= new BitList();
//         //     this.SpecialInterceptLabels?.Add(list);
//         // }
//         //
//         // public virtual void AddSpecialKeepLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialKeepLabels ??= new BitList();
//         //     this.SpecialKeepLabels?.Add(list);
//         // }
//         //
//         // public virtual void AddSpecialExclusionLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialExclusionLabels ??= new BitList();
//         //     this.SpecialExclusionLabels?.Add(list);
//         // }
//         //
//         // public virtual void AddSpecialRunLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialRunLabels ??= new BitList();
//         //     this.SpecialRunLabels?.Add(list);
//         // }
//         //
//         // public virtual void AddSpecialDisableLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialDisableLabels ??= new BitList();
//         //     this.SpecialDisableLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialPassedLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialPassLabels ??= new BitList();
//         //     this.SpecialPassLabels?.Clear();
//         //     this.SpecialPassLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialInterceptLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialInterceptLabels ??= new BitList();
//         //     this.SpecialInterceptLabels?.Clear();
//         //     this.SpecialInterceptLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialKeepLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialKeepLabels ??= new BitList();
//         //     this.SpecialKeepLabels?.Clear();
//         //     this.SpecialKeepLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialExclusionLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialExclusionLabels ??= new BitList();
//         //     this.SpecialExclusionLabels?.Clear();
//         //     this.SpecialExclusionLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialRunLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialRunLabels ??= new BitList();
//         //     this.SpecialRunLabels?.Clear();
//         //     this.SpecialRunLabels?.Add(list);
//         // }
//         //
//         // public virtual void SetSpecialDisableLabels(BitList list) {
//         //     if (BitList.IsNullOrEmpty(list)) return;
//         //     // this.SpecialDisableLabels ??= new BitList();
//         //     this.SpecialDisableLabels?.Clear();
//         //     this.SpecialDisableLabels?.Add(list);
//         // }
//         //
//         // public virtual void RemoveSpecialPassedLabels(BitList list) => this.SpecialPassLabels?.Remove(list);
//         // public virtual void RemoveSpecialInterceptLabels(BitList list) => this.SpecialInterceptLabels?.Remove(list);
//         // public virtual void RemoveSpecialKeepLabels(BitList list) => this.SpecialKeepLabels?.Remove(list);
//         // public virtual void RemoveSpecialExclusionLabels(BitList list) => this.SpecialExclusionLabels?.Remove(list);
//         // public virtual void RemoveSpecialRunLabels(BitList list) => this.SpecialRunLabels?.Remove(list);
//         // public virtual void RemoveSpecialDisableLabels(BitList list) => this.SpecialDisableLabels?.Remove(list);
//
//
//         // 数据存储代码（实际使用下来，发现使用频率并不高，所以干脆去掉，节省内存）
//
//         // protected Dictionary<string, object> _dataDict;
//         // public IReadOnlyDictionary<string, object> ReadOnlyDataDict => this._dataDict;
//
//         // public object this[string key] {
//         //     get {
//         //         if (this._dataDict == null) {
//         //             throw new Exception("<数据为空，请先设置数据，再获得>");
//         //         }
//         //
//         //         return this._dataDict[key];
//         //     }
//         //     set {
//         //         this._dataDict ??= new Dictionary<string, object>();
//         //         this._dataDict[key] = value;
//         //     }
//         // }
//         //
//         // public void AddData(string key, object value) {
//         //     this._dataDict ??= new Dictionary<string, object>();
//         //     this._dataDict.Add(key, value);
//         // }
//         //
//         // public bool RemoveData(string key) {
//         //     if (this._dataDict == null) {
//         //         return false;
//         //     }
//         //
//         //     return this._dataDict.Remove(key);
//         // }
//         //
//         // public bool TryGetData(string key, out object result) {
//         //     if (this._dataDict == null) {
//         //         result = null;
//         //         return false;
//         //     }
//         //
//         //     return this._dataDict.TryGetValue(key, out result);
//         // }
//         //
//         // public bool ContainsData(string key) {
//         //     if (this._dataDict == null) {
//         //         return false;
//         //     }
//         //
//         //     return this._dataDict.ContainsKey(key);
//         // }
//         //
//         // public bool ContainsData(object value) {
//         //     if (this._dataDict == null) {
//         //         return false;
//         //     }
//         //
//         //     return this._dataDict.ContainsValue(value);
//         // }
//
//         #endregion
//     }
// }

