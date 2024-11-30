namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureChangeScene : AProcedureState {
        [ShadowFunction]
        protected override async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await this.OnEnterShadow(fsm, prev);
            var data = this.GetData<(string sceneName, LoadSceneMode loadSceneMode)>();
            await SceneManager.LoadSceneWithUnity(data.sceneName, data.loadSceneMode);
        }

        [ShadowFunction]
        protected override HTask OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
            return default;
        }
    }
}