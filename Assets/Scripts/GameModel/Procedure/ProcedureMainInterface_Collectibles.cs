namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface_Collectibles : AProcedureState<ProcedureMainInterface> {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
        }
    }
}