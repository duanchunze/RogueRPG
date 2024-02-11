using System;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureShopping : AProcedureState {
        public Action onShoppingFinish;

        [ShadowFunction]
        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            this.OnEnterShadow(manager, prev);
        }

        [ShadowFunction]
        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.onShoppingFinish = null;
            this.OnLeaveShadow(manager, next);
        }

        public void OnShoppingFinish() {
            try {
                this.onShoppingFinish?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}