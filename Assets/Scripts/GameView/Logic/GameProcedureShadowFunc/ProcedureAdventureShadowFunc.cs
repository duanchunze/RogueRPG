namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure))]
    public static partial class ProcedureAdventureShadowFunc {
        [ShadowFunction]
        private static void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            bar.HideAbilityAssist();
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UICardBar>();
        }
    }
}