// using System;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
//
// namespace ET {
//     /* 优先级处理系统
//      * 类似无限状态机, 状态个数不确定, 可自行退出
//      * 基于优先级与通道
//      * Base 都没有声明字段, 继承者自行决定是否实现. Base 更像是一个写好了完整逻辑的模板, 但不会强迫子类继承字段, 也不会强迫继承函数
//      *
//      * 思路:
//      * 接口只能定义函数, 不能写逻辑, 所以定义成抽象类, 要写逻辑则不可避免的要声明字段, 所以把字段都写为属性形式, 子类根据情况, 可以自行对基类的字段删减修改, 而不用担心会继承了不需要的字段
//      * 把所有的内容都定义为 internal, 然后就可以在第二层继承, 由我们随意的决定, 每个字段, 每个方法, 是继续 internal 还是 public 还是 private, 这样, 在其他程序集使用该组件的时候, 就
//      * 可以实现隐藏掉某些函数
//      */
//     public abstract class Priorities : Component {
//         internal abstract float InternalTimeScale { get; set; }
//         internal abstract Dictionary<string, PriorityState> InternalStates { get; }
//         internal abstract MultiList<int, PriorityState> InternalAisles { get; }
//         internal abstract Queue<int> InternalDirtyFlags { get; }
//         internal abstract PriorityState InternalCurrentEnterState { get; set; }
//
//         // 几个临时缓存
//         internal abstract List<PriorityState> InternalExclusionsCache { get; }
//         internal abstract List<PriorityState> InternalHighestDisablesCache { get; }
//         internal abstract BitList InternalSpecialDisableLabelsCache { get; }
//         internal abstract Stack<PriorityState> InternalPollingCache { get; } // 用于轮询的临时缓存
//         internal abstract Dictionary<int, PriorityState> InternalDefaults { get; }
//
//
//         /// <summary>
//         /// 细节判存
//         /// </summary>
//         /// <param name="state"></param>
//         /// <param name="same">名字相同, 或者完全相同</param>
//         /// <returns>0代表不存在, 1代表存在一个同名的但不是本人, 2代表存在且是本人</returns>
//         internal virtual byte InternalContainsDetails(PriorityState state, out PriorityState same) {
//             if (this.InternalStates.TryGetValue(state.InternalName, out same)) {
//                 if (state == same) {
//                     return 2;
//                 }
//
//                 return 1;
//             }
//
//             return 0;
//         }
//
//         internal virtual bool InternalEnter(PriorityState state, PriorityStateEnterFailDetails details) {
//             if (state.InternalManager != null) {
//                 if (state.InternalManager != this) {
//                     throw new Exception($"<一个状态同时只能进入一个状态机> '{state.InternalName}'");
//                 }
//             }
//
//             var @case = this.InternalContainsDetails(state, out var subrogee);
//             switch (@case) {
//                 // 如果是完全相同一个状态要再次进入视为重进
//                 case 2:
//                     // 不需要再次和通道内的对比，直接和自己对比，条件满足，则进入成功，并调用进入事件
//                     if (state.InternalEnterPriority > state.InternalResistPriority) {
//                         // 做做样子，tree本地并没有做任何修改
//                         state.InternalOnLeave(this);
//                         state.InternalOnLeaveDetails(this, PriorityStateLeaveType.ReEnter, null);
//                         state.InternalOnEnter(this);
//                         return true;
//                     }
//
//                     if (details != null) {
//                         details.failType = PriorityStateEnterFailType.PriorityLow;
//                         details.blocker = state;
//                     }
//
//                     break;
//
//                 // 同名时，视为替换。除了必然会移除之前的状态外，其他的和进入一个全新的状态没有差别
//                 case 1:
//                     if (this.InternalCheckEnter(state, details)) {
//                         this.InternalRemove(subrogee, PriorityStateLeaveType.Replace, state);
//                         this.InternalAdd(state);
//                         this.InternalCheckExclusion(state);
//                         state.InternalOnEnter(this);
//                         if (!state.InternalIsEntered) {
//                             // 自己排挤别的人时候，别人在Leave回调中又导致进入了别的状态，排挤掉了自己，这种情况，就不往下执行了
//                             return true;
//                         }
//
//                         try {
//                             this.OnStateChanged(state, true);
//                         }
//                         catch (Exception e) {
//                             Log.Error(e);
//                         }
//
//                         return true;
//                     }
//
//                     break;
//
//                 // 全新的状态，走正常流程
//                 case 0:
//                     // 先检测是否够条件进入
//                     if (this.InternalCheckEnter(state, details)) {
//                         // 先把状态加入
//                         this.InternalAdd(state);
//                         // 然后把它能排挤掉的状态排挤掉
//                         this.InternalCheckExclusion(state);
//                         // 禁掉能禁用的其他状态
//                         state.InternalOnEnter(this);
//                         if (!state.InternalIsEntered) {
//                             return true;
//                         }
//
//                         try {
//                             this.OnStateChanged(state, true);
//                         }
//                         catch (Exception e) {
//                             Log.Error(e);
//                         }
//
//                         return true;
//                     }
//
//                     break;
//             }
//
//             return false;
//         }
//
//         internal virtual bool InternalRehearsalEnter(PriorityState state, PriorityStateEnterFailDetails details) {
//             if (state.InternalManager != null) {
//                 if (state.InternalManager != this) {
//                     throw new Exception($"<一个状态同时只能进入一个状态机> '{state.InternalName}'");
//                 }
//             }
//
//             var @case = this.InternalContainsDetails(state, out var _);
//             switch (@case) {
//                 // 如果是完全相同一个状态要再次进入视为重进
//                 case 2:
//                     // 不需要再次和通道内的对比，直接和自己对比，条件满足，则进入成功，并调用进入事件
//                     if (state.InternalEnterPriority > state.InternalResistPriority) {
//                         return true;
//                     }
//
//                     if (details != null) {
//                         details.failType = PriorityStateEnterFailType.PriorityLow;
//                         details.blocker = state;
//                     }
//
//                     break;
//
//                 // 同名时，视为替换。除了必然会移除之前的状态外，其他的和进入一个全新的状态没有差别
//                 case 1:
//                     if (this.InternalCheckEnter(state, details)) {
//                         return true;
//                     }
//
//                     break;
//
//                 // 全新的状态，走正常流程
//                 case 0:
//                     // 先检测是否够条件进入
//                     if (this.InternalCheckEnter(state, details)) {
//                         return true;
//                     }
//
//                     break;
//             }
//
//             return false;
//         }
//
//         internal virtual bool InternalLeave(PriorityState state) {
//             if (state == null) {
//                 Log.Error("状态为空!");
//                 return false;
//             }
//
//             if (!this.InternalStates.ContainsValue(state)) {
//                 return false;
//             }
//
//             this.InternalRemove(state, PriorityStateLeaveType.InitiativeInvoke, null);
//             return true;
//         }
//
//         /// <summary>
//         /// 1、刷新禁用
//         /// 2、补充空通道
//         /// </summary>
//         internal virtual void InternalHandleDirtiness() {
//             // 把count单独出来，该帧内再出现的新的脏数据，顺留到下帧处理，防止出现死循环的可能
//             var dirtyCount = this.InternalDirtyFlags.Count;
//             while (dirtyCount > 0) {
//                 dirtyCount--;
//                 var aisle = this.InternalDirtyFlags.Dequeue();
//
//                 #region 处理暂停相关
//
//                 // 当运行 >= 禁用的时候，就可以运行
//                 // 只要被特别禁用，那么必定会被禁用
//                 // 所有禁用权限比你大的人，只要有一个没有指定让你运行，都不可以运行
//                 // 特别禁用的优先级比特别运行的优先级高
//                 // 运行与禁用的等级最低都是0，而禁用0也代表着该状态不想禁用其他状态
//                 var maxDisablePriority = 0;
//                 this.InternalHighestDisablesCache.Clear();
//                 this.InternalSpecialDisableLabelsCache.Clear();
//                 // 从这些改变过的通道里，遍历找出最大的禁用状态
//                 if (this.InternalAisles.TryGetValue(aisle, out var list)) {
//                     if (list.Count > 1) {
//                         foreach (var state in list) {
//                             // 先把所有状态的暂停关闭。
//                             state.InternalPaused = false;
//
//                             // 把所有状态指定要禁用的标签都缓存下来
//                             if (state.InternalSpecialDisableLabels != null) {
//                                 this.InternalSpecialDisableLabelsCache.Add(state.InternalSpecialDisableLabels);
//                             }
//
//                             // 找出禁用优先级最大的状态
//                             if (state.InternalDisablePriority > maxDisablePriority) {
//                                 maxDisablePriority = state.InternalDisablePriority;
//                             }
//                         }
//                     }
//                 }
//                 else {
//                     Log.Error($"<虽然记录通道发生了改变，但并没有找到该通道> {aisle}");
//                     continue;
//                 }
//
//                 // 如果个数少于2个的话，就没必要比了
//                 if (list.Count > 1) {
//                     // 把所有参与禁用的状态缓存起来，从大到小排好顺序
//                     foreach (var state in list) {
//                         // 0 表示该状态不想禁用任何其他状态
//                         if (state.InternalDisablePriority == 0) {
//                             continue;
//                         }
//
//                         this.InternalHighestDisablesCache.Add(state);
//                     }
//
//                     this.InternalHighestDisablesCache.Sort((a, b) => b.InternalDisablePriority.CompareTo(a.InternalDisablePriority)); // 从大到小
//
//                     foreach (var state in list) {
//                         // 如果该状态已经暂停了，说明它已经在其他通道被暂停过了，所以就不需要再次判断了
//                         if (state.InternalPaused) {
//                             continue;
//                         }
//
//                         // 首先判断是不是该状态是不是被特别指定要禁用的
//                         if (this.InternalSpecialDisableLabelsCache.ContainsAny(state.InternalLabels)) {
//                             state.InternalPaused = true;
//                             continue;
//                         }
//
//                         // 如果运行等级比最高禁用等级，则直接跳过
//                         if (state.InternalRunPriority >= maxDisablePriority) {
//                             continue;
//                         }
//
//                         // 否则就和每一个禁用者对比
//                         foreach (var comparer in this.InternalHighestDisablesCache) {
//                             // 自己不会禁用自己
//                             if (comparer == state) {
//                                 continue;
//                             }
//
//                             // 一旦有一个没比过，且也没被对方指定运行，则直接暂停并中断比较
//                             if (state.InternalRunPriority < comparer.InternalDisablePriority) {
//                                 if (!comparer.InternalContainsSpecialRunOfLabels(state.InternalLabels)) {
//                                     state.InternalPaused = true;
//                                     break;
//                                 }
//                             }
//                         }
//                     }
//                 }
//
//                 #endregion
//
//                 #region 处理空通道相关
//
//                 // 如果该通道是空的，看是否有默认状态可以补充该通道
//                 if (list.Count == 0) {
//                     if (this.InternalDefaults != null) {
//                         if (this.InternalDefaults.TryGetValue(aisle, out var defaultFsmStateIntricate)) {
//                             defaultFsmStateIntricate.Enter(this);
//                         }
//                     }
//                 }
//
//                 #endregion
//             }
//         }
//
//         internal virtual void InternalAdd(PriorityState state) {
//             this.InternalStates.Add(state.InternalName, state);
//
//             foreach (var aisle in state.InternalAisles) {
//                 if (this.InternalAisles.TryGetValue(aisle, out var list)) {
//                     list.Add(state);
//                     // 进行排序，把 nowPriority 大的放在前面
//                     // states.Sort(); // 不需要排序，因为状态在运行的过程中，nowPriority可能会变的，排序也没大用
//                 }
//                 else {
//                     this.InternalAisles.Add(aisle, state);
//                 }
//
//                 if (!this.InternalDirtyFlags.Contains(aisle)) {
//                     this.InternalDirtyFlags.Enqueue(aisle);
//                 }
//             }
//
//             state.InternalManager = this;
//             this.InternalCurrentEnterState = state;
//         }
//
//         internal virtual void InternalRemove(PriorityState state, PriorityStateLeaveType leaveType, PriorityState initiator) {
//             this.InternalStates.Remove(state.InternalName);
//
//             foreach (var aisle in state.InternalAisles) {
//                 if (this.InternalAisles.TryGetValue(aisle, out var list)) {
//                     if (!list.Remove(state)) {
//                         Log.Error($"<从通道中移除状态出现异常，通道不包含该状态> 异常状态:'{state.InternalName}'，通道名称：'{aisle}'");
//                     }
//                 }
//                 else {
//                     Log.Error($"<从通道中移除状态出现异常，不包含该通道> 异常状态：'{state.InternalName}'，通道名称：'{aisle}'");
//                 }
//
//                 if (!this.InternalDirtyFlags.Contains(aisle)) {
//                     this.InternalDirtyFlags.Enqueue(aisle);
//                 }
//             }
//
//             state.InternalManager = null;
//             if (this.InternalCurrentEnterState == state) {
//                 this.InternalCurrentEnterState = null;
//             }
//
//             state.InternalOnLeave(this);
//             state.InternalOnLeaveDetails(this, leaveType, initiator);
//             try {
//                 this.OnStateChanged(state, false);
//             }
//             catch (Exception e) {
//                 Log.Error(e);
//             }
//         }
//
//         /// <summary>
//         /// 检查进入
//         /// </summary>
//         /// <param name="state"></param>
//         /// <param name="details"></param>
//         /// <returns></returns>
//         internal virtual bool InternalCheckEnter(PriorityState state, PriorityStateEnterFailDetails details) {
//             // 遍历所有当前状态想要进入的通道，并和通道里的所有状态一一对比，全部通过，才判定为可以进入。
//             var collection = state.InternalAisles;
//             var multiAisle = collection.Count > 1;
//             // if (multiAisle) this.ContrastedCache.Clear();
//             if (multiAisle) this.InternalResetHandledTokens();
//             // 遍历要进入的状态的通道
//             foreach (var aisle in collection) {
//                 // 如果该状态机里没有该通道，则可以认为该状态是该通道的第一个，那必然可进入
//                 if (!this.InternalAisles.TryGetValue(aisle, out var states)) {
//                     continue;
//                 }
//
//                 // 遍历该通道里所有的状态
//                 foreach (var other in states) {
//                     // 如果该状态已经存在于临时区，则继续
//                     // if (multiAisle && this.ContrastedCache.Contains(other)) continue;
//                     if (multiAisle && other.InternalHandledFlag) continue;
//
//                     // 获得这个对比者，用它和要进入的状态做对比
//                     if (!this.InternalContrastEnter(state, other, details)) {
//                         return false;
//                     }
//
//                     // 把对比者加入缓存区，之后便不需要再次和它对比
//                     if (multiAisle) other.InternalHandledFlag = true;
//                 }
//             }
//
//             return true;
//         }
//
//         /// <summary>
//         /// 对比状态进入相关
//         /// </summary>
//         /// <param name="source"></param>
//         /// <param name="contrast"></param>
//         /// <param name="details"></param>
//         /// <returns>true 代表 source 相对于 contrast 可以进入</returns>
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal virtual bool InternalContrastEnter(PriorityState source, PriorityState contrast, PriorityStateEnterFailDetails details) {
//             // 进入优先级 > 对比者
//             if (source.InternalEnterPriority > contrast.InternalResistPriority) {
//                 // 如果被对比者点名拦截或者点名标签拦截，则依然返回 false
//                 var result = !contrast.InternalContainsSpecialInterceptOfLabels(source.InternalLabels);
//                 if (!result) {
//                     if (details != null) {
//                         details.failType = PriorityStateEnterFailType.SpecialIntercept;
//                         details.blocker = contrast;
//                     }
//                 }
//
//                 return result;
//             }
//             else {
//                 // 如果被对比者点名通过或者点名标签通过，则依然返回 true
//                 var result = contrast.InternalContainsSpecialPassedOfLabels(source.InternalLabels);
//                 if (!result) {
//                     if (details != null) {
//                         details.failType = PriorityStateEnterFailType.PriorityLow;
//                         details.blocker = contrast;
//                     }
//                 }
//
//                 return result;
//             }
//         }
//
//         /// <summary>
//         /// 检查排挤
//         /// </summary>
//         /// <param name="state"></param>
//         internal virtual void InternalCheckExclusion(PriorityState state) {
//             // 遍历对比者要对比的所有通道，和每个通道里的所有状态做对比，把能排挤掉的都排挤掉
//             var collection = state.InternalAisles;
//             var multiAisle = collection.Count > 1;
//             this.InternalExclusionsCache.Clear();
//             // if (multiAisle) this.ContrastedCache.Clear();
//             if (multiAisle) this.InternalResetHandledTokens();
//             // 遍历对比者要求的所有通道
//             foreach (var aisle in collection) {
//                 if (!this.InternalAisles.TryGetValue(aisle, out var states)) {
//                     // 没有对应的通道很正常，状态机创建一开始，通道是空的，会根据进入的状态来拓展通道
//                     continue;
//                 }
//
//                 // 遍历通道里的每个状态
//                 foreach (var other in states) {
//                     // 如果该状态已经被排挤过了
//                     // if (multiAisle && this.ContrastedCache.Contains(other)) continue;
//                     if (multiAisle && other.InternalHandledFlag) continue;
//
//                     if (this.InternalContrastExclusion(other, state)) {
//                         this.InternalExclusionsCache.Add(other);
//                     }
//
//                     // if (multiAisle) this.ContrastedCache.Add(other);
//                     if (multiAisle) other.InternalHandledFlag = true;
//                 }
//             }
//
//             for (int i = 0, len = this.InternalExclusionsCache.Count; i < len; i++) {
//                 this.InternalRemove(this.InternalExclusionsCache[i], PriorityStateLeaveType.Exclusion, state);
//             }
//         }
//
//         /// <summary>
//         /// 对比状态排挤相关
//         /// </summary>
//         /// <param name="source"></param>
//         /// <param name="contrast"></param>
//         /// <returns>true 代表 source 会被 contrast 排挤掉</returns>
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal virtual bool InternalContrastExclusion(PriorityState source, PriorityState contrast) {
//             // 自己不会排挤自己
//             if (source == contrast) {
//                 return false;
//             }
//
//             // 自己的保持优先级 >= 对比者的排挤优先级, 代表 source 不会被排挤掉
//             if (source.InternalKeepPriority >= contrast.InternalExclusionPriority) {
//                 // 如果自己被对比者点名或标签排挤，则依然返回 true
//                 return contrast.InternalContainsSpecialExclusionOfLabels(source.InternalLabels);
//             }
//             else {
//                 // 如果自己被对比者点名或标签保持，则依然返回 false
//                 return !contrast.InternalContainsSpecialKeepOfLabels(source.InternalLabels);
//             }
//         }
//
//         [MethodImpl(MethodImplOptions.AggressiveInlining)]
//         internal virtual void InternalResetHandledTokens() {
//             foreach (var state in this.InternalStates.Values) {
//                 state.InternalHandledFlag = false;
//             }
//         }
//
//         // 外部调用, 以正常运行
//         public virtual void Update(float deltaTime) {
//             if (this.InternalDirtyFlags.Count > 0) {
//                 this.InternalHandleDirtiness();
//             }
//
//             // timeScale 不允许为小于0，不然倒着跑了
//             if (this.InternalTimeScale < 0) {
//                 this.InternalTimeScale = 0;
//             }
//
//             var finalDeltaTime = deltaTime * this.InternalTimeScale;
//
//             this.InternalPollingCache.Clear();
//             foreach (var state in this.InternalStates.Values) {
//                 this.InternalPollingCache.Push(state);
//             }
//
//             while (this.InternalPollingCache.Count != 0) {
//                 var state = this.InternalPollingCache.Pop();
//                 byte ret = 0;
//                 try {
//                     ret = state.InternalRun(finalDeltaTime);
//                 }
//                 catch (Exception e) {
//                     Log.Error(e);
//                 }
//
//                 switch (ret) {
//                     case 0: // 代表正常走
//                         continue;
//                     case 1: // 结束，移除该状态
//                         this.InternalRemove(state, PriorityStateLeaveType.TimeOut, null);
//                         break;
//                 }
//             }
//         }
//
//         protected virtual void OnStateChanged(PriorityState state, bool isEnter) { }
//
//         /// <summary>
//         /// 设置一个默认的行为，当该通道没有行为的时候，会使用该默认行为，一般使用在主通道里
//         /// </summary>
//         /// <param name="aisle">一般使用0代表主通道</param>
//         /// <param name="state"></param>
//         public virtual void SetDefaultState(int aisle, PriorityState state) {
//             if (this.InternalDefaults == null) return;
//             this.InternalDefaults[aisle] = state;
//             if (!this.InternalAisles.TryGetValue(aisle, out var list)) {
//                 state.Enter(this);
//                 return;
//             }
//
//             if (list.Count == 0) {
//                 state.Enter(this);
//             }
//         }
//         
//         public virtual void SetDefaultState(PriorityState state) {
//             foreach (var aisle in state.InternalAisles) {
//                 this.SetDefaultState(aisle, state);
//             }
//         }
//
//         public virtual PriorityState GetDefaultState(int aisle) {
//             if (this.InternalDefaults == null) return null;
//             if (this.InternalDefaults.TryGetValue(aisle, out var result)) {
//                 return result;
//             }
//
//             return null;
//         }
//
//         public virtual void RemoveDefaultState(int aisle, PriorityState state) {
//             if (this.InternalDefaults == null) return;
//             if (!this.InternalDefaults.TryGetValue(aisle, out var result)) return;
//             if (result != state) return;
//             this.InternalDefaults.Remove(aisle);
//         }
//
//         public virtual void RemoveDefaultState(PriorityState state) {
//             foreach (var aisle in state.InternalAisles) {
//                 this.RemoveDefaultState(aisle, state);
//             }
//         }
//     }
// }

