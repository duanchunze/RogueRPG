using Hsenl.View;

namespace Hsenl {
    public class ProcedureAdventure : AProcedureState {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            bar.HideAbilityAssist();
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UICardBar>();
        }
    }
}