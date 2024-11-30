namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureMainInterface))]
    public static partial class ProcedureMainInterface_Shadow {
        [ShadowFunction]
        private static async Hsenl.HTask OnEnter(ProcedureMainInterface self, Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            await HTask.Completed;
            UIManager.SingleOpen<UINavigationBar>(UILayer.High);
        }

        [ShadowFunction]
        private static HTask OnLeave(IFsm fsm, IFsmState next) {
            UIManager.SingleClose<UINavigationBar>();
            return default;
        }
    }
}