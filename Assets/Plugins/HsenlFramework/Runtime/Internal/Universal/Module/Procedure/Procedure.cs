using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sirenix.OdinInspector;

namespace Hsenl {
    public class Procedure : IFsm {
        private readonly Dictionary<Type, IFsmState> _states = new(); // key: stateType
        private readonly Dictionary<Type, Group> _groups = new(); // key: groupType

        public void RegisterProcedureState(Type type) {
            var o = Activator.CreateInstance(type);
            if (o is not IFsmState state) {
                throw new Exception($"Obj is not ProcedureState '{type.FullName}'");
            }

            this._states.Add(type, state);
            var group = this.GetOrCreateGroup(state);
            group.procedureStates.Add(type, state);
            state.Init(this);
        }

        public void RegisterProcedureStates(IEnumerable<Type> types) {
            foreach (var type in types) {
                this.RegisterProcedureState(type);
            }
        }

        public void Update(float deltaTime) {
            foreach (var kv in this._groups) {
                kv.Value.currentState?.Update(this, deltaTime);
            }
        }

        public bool ChangeState<T>() where T : IFsmState {
            var state = this.GetState<T>();
            return this.ChangeState(state, null);
        }

        public bool ChangeState<T>(object data) where T : IFsmState {
            var state = this.GetState<T>();
            return this.ChangeState(state, data);
        }

        public bool ChangeState(Type type) {
            var state = this.GetState(type);
            return this.ChangeState(state, null);
        }

        public bool ChangeState(Type type, object data) {
            var state = this.GetState(type);
            return this.ChangeState(state, data);
        }

        private bool ChangeState(IFsmState fsmState, object data) {
            var group = this.GetGroup(fsmState);
            if (group == null) {
                return false;
            }

            if (this._states.TryGetValue(group.GroupType, out var value)) {
                this.ChangeState(value, null);
            }

            return this.ChangeState(group, fsmState, data);
        }

        private bool ChangeState(Group group, IFsmState fsmState, object data) {
            if (group.currentState == fsmState)
                return false;

            fsmState.SetData(data);

            var prevState = group.currentState;
            group.currentState = fsmState;
            // 如果离开的状态还是一个组的话, 则把其下所有的当前状态都离开
            if (prevState != null) {
                prevState.Leave(this, fsmState);
                if (this._groups.TryGetValue(prevState.GetType(), out var subGroup))
                    LeaveSubGroups(subGroup);
            }

            Log.Info($"Enter Procedure: {fsmState.GetType().Name}(Group: {group.GroupType.Name})");
            fsmState.Enter(this, prevState);

            return true;

            void LeaveSubGroups(Group sub) {
                if (sub.currentState != null) {
                    sub.currentState.Leave(this, null);
                    sub.currentState = null;
                }
                
                foreach (var kv in sub.procedureStates) {
                    if (!this._groups.TryGetValue(kv.Value.GetType(), out var g))
                        continue;

                    LeaveSubGroups(g);
                }
            }
        }

        public T GetState<T>() where T : IFsmState {
            var type = typeof(T);
            var state = this.GetState(type);
            return (T)state;
        }

        public IFsmState GetState(Type type) {
            if (!this._states.TryGetValue(type, out var state)) {
                throw new Exception($"Procedure manager do not contain state '{type.FullName}'");
            }

            return state;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Group GetGroup(IFsmState state) {
            if (state is not ProcedureState procedureState)
                return null;

            var groupType = procedureState.Group ?? typeof(ProcedureState);

            return this._groups[groupType];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Group GetOrCreateGroup(IFsmState state) {
            var groupType = (state as ProcedureState)?.Group;
            groupType ??= typeof(ProcedureState);
            if (!this._groups.TryGetValue(groupType, out var group)) {
                group = new Group {
                    GroupType = groupType
                };
                this._groups[groupType] = group;
            }

            return group;
        }

        private class Group {
            public Type GroupType { get; set; }

#if UNITY_EDITOR
            [ShowInInspector]
#endif
            public readonly Dictionary<Type, IFsmState> procedureStates = new();

#if UNITY_EDITOR
            [ShowInInspector]
#endif
            public IFsmState currentState;
        }
    }
}