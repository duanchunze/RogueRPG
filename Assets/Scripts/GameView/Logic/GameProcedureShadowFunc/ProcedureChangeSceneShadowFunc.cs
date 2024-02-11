namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureChangeScene))]
    public static partial class ProcedureChangeSceneShadowFunc {
        [ShadowFunction]
        private static async ETTask OnEnter(ProcedureChangeScene self, ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UISceneLoading>(UILayer.High);
            await ETTask.CompletedTask;
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureChangeScene self, ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UISceneLoading>();
        }
    }
}