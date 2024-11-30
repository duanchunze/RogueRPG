namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure_ChangeScene : AProcedureState<ProcedureAdventure> {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await this.OnEnterShadow(fsm, prev);
            var data = this.GetData<(string sceneName, LoadSceneMode loadSceneMode)>();
            await SceneManager.LoadSceneWithUnity(data.sceneName, data.loadSceneMode);
        }

        [ShadowFunction]
        protected override async HTask OnLeave(IFsm fsm, IFsmState next) {
            await this.OnLeaveShadow(fsm, next);
        }
    }
}