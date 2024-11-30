using System;

namespace Hsenl {
    public abstract class AProcedureState : ProcedureState {
        // 是否在指定小组中
        public bool IsInGroup<T>() {
            var type = typeof(T);
            var state = this;
            while (state != null) {
                if (state is T)
                    return true;

                if (state.Group == null)
                    return false;

                if (state.Group == type)
                    return true;

                state = ProcedureManager.Procedure.GetState(state.Group) as AProcedureState;
            }

            return false;
        }
    }

    public abstract class AProcedureState<T> : AProcedureState where T : ProcedureState {
        public sealed override Type Group => typeof(T);
    }
}