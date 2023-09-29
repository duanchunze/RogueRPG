// using System;
// using System.Collections.Generic;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace ET {
//     [Serializable]
//     public class DefaultPriorities : Priorities, IAwake, IDisposable {
//         [SerializeField, PropertyRange(0f, 5f), LabelText("时间缩放")]
//         protected float timeScale = 1f;
//
//         [ShowInInspector, ReadOnly, LabelText("所有状态")] protected Dictionary<string, PriorityState> states = new();
//         [ShowInInspector, ReadOnly, LabelText("所有通道")] protected MultiList<int, PriorityState> aisles = new();
//         [ShowInInspector, ReadOnly, LabelText("脏数据标记")] protected Queue<int> dirtyFlags = new();
//         [ShowInInspector, ReadOnly, LabelText("当前进入的状态")] protected PriorityState currentEnterState;
//         [ShowInInspector, ReadOnly] protected List<PriorityState> exclusionsCache = new();
//         [ShowInInspector, ReadOnly] protected List<PriorityState> highestDisablesCache = new();
//         [ShowInInspector, ReadOnly] protected BitList specialDisableLabelsCache = new();
//         [ShowInInspector, ReadOnly] protected Stack<PriorityState> pollingCache = new();
//         [ShowInInspector, ReadOnly, LabelText("默认状态")] protected Dictionary<int, PriorityState> defaults = new();
//
//         void IAwake.Awake() {
//             
//         }
//
//         public virtual void Dispose() {
//             this.timeScale = 1f;
//             this.currentEnterState = null;
//             this.states?.Clear();
//             this.aisles?.Clear();
//             this.dirtyFlags?.Clear();
//             this.exclusionsCache?.Clear();
//             this.highestDisablesCache?.Clear();
//             this.specialDisableLabelsCache?.Clear();
//             this.pollingCache?.Clear();
//             this.defaults?.Clear();
//         }
//
//         internal override float InternalTimeScale {
//             get => this.timeScale;
//             set => this.timeScale = value;
//         }
//
//         internal override Dictionary<string, PriorityState> InternalStates => this.states;
//
//         internal override MultiList<int, PriorityState> InternalAisles => this.aisles;
//
//         internal override Queue<int> InternalDirtyFlags => this.dirtyFlags;
//
//         internal override PriorityState InternalCurrentEnterState {
//             get => this.currentEnterState;
//             set => this.currentEnterState = value;
//         }
//
//         internal override List<PriorityState> InternalExclusionsCache => this.exclusionsCache;
//
//         internal override List<PriorityState> InternalHighestDisablesCache => this.highestDisablesCache;
//
//         internal override BitList InternalSpecialDisableLabelsCache => this.specialDisableLabelsCache;
//
//         internal override Stack<PriorityState> InternalPollingCache => this.pollingCache;
//
//         internal override Dictionary<int, PriorityState> InternalDefaults => this.defaults;
//     }
// }

