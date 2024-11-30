namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure_Settlement))]
    public static partial class ProcedureAdventure_Settlement_Shadow {
        [ShadowFunction]
        private static async HTask OnEnter(IFsm fsm, IFsmState prev) {
            await HTask.Completed;
            UIManager.SingleOpen<UIAdvSettlement>(UILayer.High);
        }

        [ShadowFunction]
        private static HTask OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UIAdvSettlement>();
            return default;
        }
    }
}