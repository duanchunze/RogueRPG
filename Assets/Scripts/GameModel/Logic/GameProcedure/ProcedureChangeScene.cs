namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureChangeScene : AProcedureState<(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)> {
        [ShadowFunction]
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            this.OnEnterShadow(manager, prev);
            await SceneManager.LoadSceneWithUnity(this.data.sceneName, this.data.loadSceneMode);
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.OnLeaveShadow(manager, next);
        }
    }
}