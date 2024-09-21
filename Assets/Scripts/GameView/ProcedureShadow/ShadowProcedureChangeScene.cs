namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureChangeScene))]
    public static partial class ShadowProcedureChangeScene {
        [ShadowFunction]
        private static async Hsenl.HTask OnEnter(Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            UIManager.SingleOpen<UISceneLoading>(UILayer.High);
            await HTask.Completed;
        }

        [ShadowFunction]
        private static void OnLeave(Hsenl.IFsm fsm, Hsenl.IFsmState next) {
            UIManager.SingleClose<UISceneLoading>();
        }
    }
}