using Hsenl.View;

namespace Hsenl {
    public class ProcedureChangeScene : AProcedureState<(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode)> {
        protected override async void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UISceneLoading>(UILayer.High);
            await SceneManager.LoadSceneWithUnity(this.data.sceneName, this.data.loadSceneMode);
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UISceneLoading>();
        }
    }
}