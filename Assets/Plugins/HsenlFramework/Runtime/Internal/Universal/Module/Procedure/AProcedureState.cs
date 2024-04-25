using System;

namespace Hsenl {
    public abstract class AProcedureState : ProcedureState { }

    public abstract class AProcedureState<T> : AProcedureState where T : ProcedureState {
        public sealed override Type Group => typeof(T);
    }
}