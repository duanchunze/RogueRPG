using System;
using System.Collections.Generic;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class Prioritizer : Unbodied, IPrioritizer, IUpdate {
#if UNITY_EDITOR
        [SerializeField, PropertyRange(0f, 5f), LabelText("时间缩放")]
#endif
        [MemoryPackInclude]
        public float timeScale = 1f;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("所有状态"), DictionaryDrawerSettings(KeyLabel = "状态名", ValueLabel = "状态")]
#endif
        [MemoryPackIgnore]
        protected HashSet<IPriorityState> states = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("所有通道"), DictionaryDrawerSettings(KeyLabel = "通道", ValueLabel = "状态们")]
#endif
        [MemoryPackIgnore]
        protected MultiList<int, IPriorityState> aisles = new();

        [MemoryPackIgnore]
        protected Queue<int> dirtyFlags = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("当前进入的状态")]
#endif
        [MemoryPackIgnore]
        protected IPriorityState currentEnterState;
        
        [MemoryPackIgnore]
        protected IPriorityState evaluateSuccessdCache;

        [MemoryPackIgnore]
        protected List<IPriorityState> exclusionsCache = new();
        [MemoryPackIgnore]
        protected List<IPriorityState> highestDisablesCache = new();
        [MemoryPackIgnore]
        protected Bitlist specialDisableLabelsCache = new();
        [MemoryPackIgnore]
        protected Stack<IPriorityState> pollingCache = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("默认状态"), DictionaryDrawerSettings(KeyLabel = "通道", ValueLabel = "状态")]
#endif
        [MemoryPackIgnore]
        protected Dictionary<int, IPriorityState> defaults = new();

        protected internal override void OnDisposed() {
            base.OnDisposed();
            foreach (var state in this.states) {
                state.Leave();
            }

            this.timeScale = 1f;
            this.states?.Clear();
            this.aisles?.Clear();
            this.dirtyFlags?.Clear();
            this.currentEnterState = null;
            this.exclusionsCache?.Clear();
            this.highestDisablesCache?.Clear();
            this.specialDisableLabelsCache?.Clear();
            this.pollingCache?.Clear();
            this.defaults?.Clear();
        }

        public void Update() {
            ((IPrioritizer)this).Update(TimeInfo.DeltaTime);
        }

        float IPrioritizer.TimeScale {
            get => this.timeScale;
            set => this.timeScale = value;
        }

        HashSet<IPriorityState> IPrioritizer.States => this.states;

        MultiList<int, IPriorityState> IPrioritizer.Aisles => this.aisles;

        Queue<int> IPrioritizer.DirtyFlags => this.dirtyFlags;

        IPriorityState IPrioritizer.CurrentEnterState {
            get => this.currentEnterState;
            set => this.currentEnterState = value;
        }

        IPriorityState IPrioritizer.EvaluateSuccessdCache {
            get => this.evaluateSuccessdCache;
            set => this.evaluateSuccessdCache = value;
        }

        List<IPriorityState> IPrioritizer.ExclusionsCache => this.exclusionsCache;

        List<IPriorityState> IPrioritizer.HighestDisablesCache => this.highestDisablesCache;

        Bitlist IPrioritizer.SpecialDisableLabelsCache => this.specialDisableLabelsCache;

        Stack<IPriorityState> IPrioritizer.PollingCache => this.pollingCache;

        Dictionary<int, IPriorityState> IPrioritizer.Defaults => this.defaults;

        public void SetDefaultPriorityState(int aisle, IPriorityState state) {
            ((IPrioritizer)this).SetDefaultState(aisle, state);
        }

        public void SetDefaultPriorityState(IPriorityState state) {
            ((IPrioritizer)this).SetDefaultState(state);
        }

        public IPriorityState GetDefaultPriorityState(int aisle) {
            return ((IPrioritizer)this).GetDefaultState(aisle);
        }

        public void RemoveDefaultPriorityState(int aisle, IPriorityState state) {
            ((IPrioritizer)this).RemoveDefaultState(aisle, state);
        }

        public void RemoveDefaultPriorityState(IPriorityState state) {
            ((IPrioritizer)this).RemoveDefaultState(state);
        }

        public bool ContainsState(string name) {
            return ((IPrioritizer)this).Contains(name);
        }

        public PriorityState GetState(string name) {
            return (PriorityState)((IPrioritizer)this).Get(name);
        }
    }
}