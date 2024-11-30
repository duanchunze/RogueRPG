namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureMainInterface : AProcedureState {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await SceneManager.LoadSceneWithUnity("GameInterface", LoadSceneMode.Single);
            await this.OnEnterShadow(fsm, prev);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
        }
    }
}