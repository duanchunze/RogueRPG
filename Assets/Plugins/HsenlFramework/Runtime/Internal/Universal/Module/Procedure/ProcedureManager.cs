using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    /// <summary>
    /// 流程系统
    /// 其实严格来说, 更像是一种游戏状态管理器
    /// 思路历程: 比如现在, 玩家在大厅, 想要开启一段冒险, 你可以有以下几种实现方案
    /// 1、有一个游戏管理器, 里面有个函数, 调用该函数, 开启一段冒险
    /// 2、发送一个Event, 在Handler里实现开启一段冒险
    /// 3、使用流程系统来切换到"开启冒险"状态, 让该状态去实现开启冒险
    /// 这三种方案都能满足需求, 但前两种存在一个问题, 当你开启一段冒险时, 无法方便处理这之前需要处理的事, 比如, 你从主界面点击开启一段冒险, 此时, 你需要把主界面的UI给关闭, 如果你
    /// 是使用前两种方案来做的话, 那你就不方便把这个UI给关掉, 你直接去关UI, 那这就产生了耦合, 你不关, 那你也不好找别的合适的人去关它. 那流程系统就没这个问题, 无论你是从主界面开启冒险,
    /// 还是从大厅开启冒险, 切换时, 之前的状态都会触发离开事件, 把关闭UI写在离开事件里就好了.
    /// </summary>
    [Serializable]
    public class ProcedureManager : Singleton<ProcedureManager>, IFsm {
#if UNITY_EDITOR
        [ShowInInspector]
#endif
        private readonly Dictionary<Type, AProcedureState> _procedureStates = new();

        private readonly Dictionary<Type, AProcedureState> _procedureStateShadows = new();

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        private AProcedureState _currentState;

        public AProcedureState CurrentState => this._currentState;

        public void RegisterProcedureStates() {
            this._procedureStates.Clear();
            var types = EventSystem.GetTypesOfAttribute(typeof(ProcedureStateAttribute));
            foreach (var type in types) {
                var state = AProcedureState.Create(this, type);
                if (state == null) continue;
                this._procedureStates.Add(type, state);
            }
        }

        public void RegisterProcedureStates(IEnumerable<Type> types) {
            this._procedureStates.Clear();
            foreach (var type in types) {
                var state = AProcedureState.Create(this, type);
                if (state == null) continue;
                this._procedureStates.Add(type, state);
            }
        }

        public void Update(float deltaTime) {
            foreach (var state in this._procedureStates.Values) {
                AProcedureState.Update(this, state, deltaTime);
            }
        }

        public bool ChangeState<T>() where T : AProcedureState {
            var type = typeof(T);
            if (!this._procedureStates.TryGetValue(type, out var state)) {
                throw new Exception($"procedure manager do not contain state '{type.FullName}'");
            }

            return this.ChangeState(state);
        }

        public bool ChangeState<T, TData>(TData data) where T : AProcedureState {
            var type = typeof(T);
            if (!this._procedureStates.TryGetValue(type, out var state)) {
                throw new Exception($"procedure manager do not contain state '{type.FullName}'");
            }

            return this.ChangeState(state, data);
        }

        public bool ChangeState(Type type) {
            if (!this._procedureStates.TryGetValue(type, out var state)) {
                throw new Exception($"procedure manager do not contain state '{type.FullName}'");
            }

            return this.ChangeState(state);
        }

        public bool ChangeState<TData>(Type type, TData data) {
            if (!this._procedureStates.TryGetValue(type, out var state)) {
                throw new Exception($"procedure manager do not contain state '{type.FullName}'");
            }

            return this.ChangeState(state, data);
        }

        public bool ChangeState<TData>(AProcedureState fsmState, TData data) {
            return this.ChangeState(fsmState, callback: state => { ((AProcedureState<TData>)state).SetData(data); });
        }

        public bool ChangeState(AProcedureState fsmState, Action<AProcedureState> callback = null) {
            if (fsmState == this._currentState)
                return false;

            var previousState = this._currentState;
            this._currentState = fsmState;
            callback?.Invoke(fsmState);
            if (previousState != null) {
                AProcedureState.Leave(this, previousState, fsmState);
            }

            Log.Info($"Enter Procedure: {fsmState.GetType().Name}");
            AProcedureState.Enter(this, fsmState, previousState);
            return true;
        }

        public T GetState<T>() where T : AProcedureState {
            var type = typeof(T);
            if (!this._procedureStates.TryGetValue(type, out var state)) {
                throw new Exception($"procedure manager do not contain state '{type.FullName}'");
            }

            return (T)state;
        }

        protected override void OnSingleUnregister() {
            if (this._currentState != null) {
                AProcedureState.Leave(this, this._currentState, null);
            }

            this._currentState = null;

            foreach (var procedureState in this._procedureStates.Values) {
                AProcedureState.Destroy(this, procedureState);
            }

            this._procedureStates.Clear();
        }
    }
}