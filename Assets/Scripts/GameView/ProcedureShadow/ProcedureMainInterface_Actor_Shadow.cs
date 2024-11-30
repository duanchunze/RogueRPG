namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureMainInterface_Actor))]
    public static partial class ProcedureMainInterface_Actor_Shadow {
        [ShadowFunction]
        private static async HTask OnEnter(ProcedureMainInterface_Actor self, IFsm fsm, IFsmState prev) {
            await HTask.Completed;
            UIManager.SingleOpen<UIActorWarehouse>(UILayer.High);
        }

        [ShadowFunction]
        private static async HTask OnLeave(IFsm fsm, IFsmState next) {
            await HTask.Completed;
            UIManager.SingleClose<UIActorWarehouse>();
        }
    }
}