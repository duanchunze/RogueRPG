namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureSelectHero))]
    public static partial class ProcedureSelectHeroShadowFunc {
        [ShadowFunction]
        private static void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UISelectHero>(UILayer.High);
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UISelectHero>();
        }
    }
}