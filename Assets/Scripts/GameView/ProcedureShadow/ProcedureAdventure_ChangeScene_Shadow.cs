namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure_ChangeScene))]
    public static partial class ProcedureAdventure_ChangeScene_Shadow {
        [ShadowFunction]
        private static async HTask OnEnter(ProcedureAdventure_ChangeScene self, IFsm fsm, IFsmState prev) {
            UIManager.SingleOpen<UISceneLoading>(UILayer.High);
            await HTask.Completed;
        }

        [ShadowFunction]
        private static HTask OnLeave(ProcedureAdventure_ChangeScene self, IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UISceneLoading>();
            return default;
        }
    }
}