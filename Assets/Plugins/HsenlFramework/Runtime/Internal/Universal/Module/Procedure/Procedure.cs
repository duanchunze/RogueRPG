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

        public async HTask<bool> ChangeState<T>() where T : IFsmState {
            var state = this.GetState<T>();
            return await this.ChangeState(state, null);
        }

        public async HTask<bool> ChangeState<T>(object data) where T : IFsmState {
            var state = this.GetState<T>();
            return await this.ChangeState(state, data);
        }

        public async HTask<bool> ChangeState(Type type) {
            var state = this.GetState(type);
            return await this.ChangeState(state, null);
        }

        public async HTask<bool> ChangeState(Type type, object data) {
            var state = this.GetState(type);
            return await this.ChangeState(state, data);
        }

        private async HTask<bool> ChangeState(IFsmState fsmState, object data) {
            var group = this.GetGroup(fsmState);
            if (group == null) {
                return false;
            }

            if (this._states.TryGetValue(group.GroupType, out var groupState)) {
                if (!groupState.IsEntering) {
                    await this.ChangeState(groupState, null);
                }
            }

            return await this.ChangeState(group, fsmState, data);
        }

        private async HTask<bool> ChangeState(Group group, IFsmState fsmState, object data) {
            if (group.currentState == fsmState)
                return false;

            if (fsmState.IsEntering)
                return false;

            fsmState.IsEntering = true;
            var prevState = group.currentState;
            group.currentState = null;
            // 如果离开的状态还是一个组的话, 则把其下所有的当前状态都离开
            if (prevState != null) {
                await Leave(prevState, fsmState);
            }

            fsmState.SetData(data);
            var now = DateTimeOffset.UtcNow;
            await fsmState.Enter(this, prevState);
            group.currentState = fsmState;
            fsmState.IsEntering = false;
            Log.Info(
                $"Enter <color=green> Procedure: {fsmState.GetType().Name}</color> (Group: {group.GroupType.Name} Time: {(DateTimeOffset.UtcNow - now).Milliseconds}ms)");

            return true;

            async HTask Leave(IFsmState state, IFsmState next) {
                if (state.IsLeaving)
                    return;

                state.IsLeaving = true;
                if (this._groups.TryGetValue(state.GetType(), out var group_)) {
                    var prev = group_.currentState;
                    group_.currentState = null;
                    if (prev != null) {
                        await Leave(prev, null);
                    }
                }

                await state.Leave(this, next);

                state.IsLeaving = false;
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