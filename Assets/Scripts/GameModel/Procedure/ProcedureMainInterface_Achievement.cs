namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface_Achievement : AProcedureState<ProcedureMainInterface> {
        [ShadowFunction]
        protected override void OnEnter(IFsm fsm, IFsmState prev) {
            this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }
    }
}