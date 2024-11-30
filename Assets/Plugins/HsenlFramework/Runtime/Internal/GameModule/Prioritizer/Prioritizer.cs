using System;
using System.Collections.Generic;
using System.Linq;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
#endif

namespace Hsenl {
    [Serializable]
    [MemoryPackable]
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
        protected Dictionary<int, IPrioritizer.Aisle> aisles = new();

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
        protected List<IPriorityState> removesCache = new();

        [MemoryPackIgnore]
        protected Stack<IPriorityState> pollingCache = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, LabelText("默认状态"), DictionaryDrawerSettings(KeyLabel = "通道", ValueLabel = "状态")]
#endif
        [MemoryPackIgnore]
        protected Dictionary<int, IPriorityState> defaults = new();

        public event Action<IPriorityState> OnStateEnter;
        public event Action<IPriorityState> OnStateLeave;

        internal override void OnDestroyInternal() {
            foreach (var state in this.states.ToArray()) {
                state.Leave_Interface();
            }
        }

        internal override void OnDisposedInternal() {
            base.OnDisposedInternal();
            this.timeScale = 1f;
            this.states?.Clear();
            this.aisles?.Clear();
            this.dirtyFlags?.Clear();
            this.currentEnterState = null;
            this.evaluateSuccessdCache = null;
            this.removesCache?.Clear();
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

        HashSet<IPriorityState> IPrioritizer._allStates => this.states;

        Dictionary<int, IPrioritizer.Aisle> IPrioritizer._aisles => this.aisles;

        Queue<int> IPrioritizer._dirtyFlags => this.dirtyFlags;

        IPriorityState IPrioritizer.CurrentEnterState {
            get => this.currentEnterState;
            set => this.currentEnterState = value;
        }

        IPriorityState IPrioritizer.EvaluateSuccessdCache {
            get => this.evaluateSuccessdCache;
            set => this.evaluateSuccessdCache = value;
        }

        List<IPriorityState> IPrioritizer._removesCache => this.removesCache;

        Stack<IPriorityState> IPrioritizer._pollingCache => this.pollingCache;

        Dictionary<int, IPriorityState> IPrioritizer._defaults => this.defaults;

        void IPrioritizer.OnStateChanged(IPriorityState state, bool isEnter) {
            if (isEnter) {
                try {
                    this.OnStateEnter?.Invoke(state);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
            else {
                try {
                    this.OnStateLeave?.Invoke(state);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

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