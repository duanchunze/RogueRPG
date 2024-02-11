namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureMainInterface))]
    public static partial class ProcedureMainInterfaceShadowFunc {
        [ShadowFunction]
        private static void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UIMainInterface>(UILayer.High);
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UIMainInterface>();
        }
    }
}