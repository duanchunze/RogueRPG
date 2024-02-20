namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureChangeScene))]
    public static partial class ProcedureChangeSceneShadowFunc {
        [ShadowFunction]
        private static async HTask OnEnter(ProcedureChangeScene self, ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UISceneLoading>(UILayer.High);
            await HTask.Completed;
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureChangeScene self, ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UISceneLoading>();
        }
    }
}