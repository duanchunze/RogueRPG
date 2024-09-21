namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure_Battle))]
    public static partial class ProcedureAdventure_Battle_Shadow {
        [ShadowFunction]
        private static void OnEnter(Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            UIManager.SingleOpen<UILvInfo>(UILayer.High);
        }

        [ShadowFunction]
        private static void OnLeave(Hsenl.IFsm fsm, Hsenl.IFsmState next) {
            UIManager.SingleClose<UILvInfo>();
        }
    }
}