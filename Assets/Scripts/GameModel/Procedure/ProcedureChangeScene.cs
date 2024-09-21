namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureChangeScene : AProcedureState {
        [ShadowFunction]
        protected override async void OnEnter(IFsm fsm, IFsmState prev) {
            this.Wait();

            this.OnEnterShadow(fsm, prev).Tail();
            var data = this.GetData<(string sceneName, LoadSceneMode loadSceneMode)>();
            await SceneManager.LoadSceneWithUnity(data.sceneName, data.loadSceneMode);

            this.Done();
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.OnLeaveShadow(fsm, next);
        }
    }
}