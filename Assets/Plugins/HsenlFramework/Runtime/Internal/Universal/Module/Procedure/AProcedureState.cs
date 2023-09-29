using System;

namespace Hsenl {
    [ProcedureState]
    public abstract class AProcedureState : FsmState<ProcedureManager> {
        internal static AProcedureState Create(ProcedureManager manager, Type type) {
            var o = Activator.CreateInstance(type);
            if (o is not AProcedureState state) {
                throw new Exception($"obj is not ProcedureState '{type.FullName}'");
            }

            try {
                state.OnInitInternal(manager);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            return state;
        }

        internal static void Enter(ProcedureManager manager, AProcedureState state, AProcedureState prev) {
            try {
                state.OnEnterInternal(manager, prev);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal static void Update(ProcedureManager manager, AProcedureState state, float deltaTime) {
            try {
                state.OnUpdateInternal(manager, deltaTime);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal static void Leave(ProcedureManager manager, AProcedureState state, AProcedureState next) {
            try {
                state.OnLeaveInternal(manager, next);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal static void Destroy(ProcedureManager manager, AProcedureState state) {
            try {
                state.OnDestroyInternal(manager);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal virtual void OnInitInternal(ProcedureManager manager) {
            this.OnInit(manager);
        }

        internal virtual void OnEnterInternal(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            this.OnEnter(manager, prev);
        }

        internal virtual void OnUpdateInternal(ProcedureManager manager, float deltaTime) {
            this.OnUpdate(manager, deltaTime);
        }

        internal virtual void OnLeaveInternal(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeave(manager, next);
        }

        internal virtual void OnDestroyInternal(ProcedureManager manager) {
            this.OnDestroy(manager);
        }

        protected override void OnInit(ProcedureManager manager) { }

        protected override void OnUpdate(ProcedureManager manager, float deltaTime) { }

        protected override void OnDestroy(ProcedureManager manager) { }
    }

    public abstract class AProcedureState<T> : AProcedureState {
        protected T data;

        internal void SetData(T t) {
            this.data = t;
        }

        internal override void OnLeaveInternal(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.data = default;
            this.OnLeave(manager, next);
        }
    }
}