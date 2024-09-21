namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure_Battle : AProcedureState<ProcedureAdventure> {
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