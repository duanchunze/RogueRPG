namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure_Battle))]
    public static partial class ProcedureAdventure_Battle_Shadow {
        [ShadowFunction]
        private static Hsenl.HTask OnEnter(Hsenl.IFsm fsm, Hsenl.IFsmState prev) {
            UIManager.SingleOpen<UILvInfo>(UILayer.High);
            return default;
        }

        [ShadowFunction]
        private static Hsenl.HTask OnLeave(Hsenl.IFsm fsm, Hsenl.IFsmState next) {
            UIManager.SingleClose<UILvInfo>();
            return default;
        }
    }
}