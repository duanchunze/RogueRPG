using Hsenl.View;

namespace Hsenl {
    public class ProcedureMainInterface : AProcedureState {
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UIMainInterface>(UILayer.High);
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UIMainInterface>();
        }
    }
}