using Hsenl.View;

namespace Hsenl {
    public class ProcedureSelectHero : AProcedureState {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UISelectHero>(UILayer.High);
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UISelectHero>();
        }
    }
}