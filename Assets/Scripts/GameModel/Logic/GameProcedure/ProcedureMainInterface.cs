namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface : AProcedureState {
        [ShadowFunction]
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            this.OnEnterShadow(manager, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeaveShadow(manager, next);
        }
    }
}