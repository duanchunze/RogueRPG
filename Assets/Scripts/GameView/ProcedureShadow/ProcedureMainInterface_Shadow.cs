namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureMainInterface))]
    public static partial class ProcedureMainInterface_Shadow {
        [ShadowFunction]
        private static void OnEnter(ProcedureMainInterface self, IFsm fsm, IFsmState prev) {
            UIManager.SingleOpen<UINavigationBar>(UILayer.High);
        }

        [ShadowFunction]
        private static void OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UINavigationBar>();
        }
    }
}