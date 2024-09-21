using System;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure_Shopping : AProcedureState<ProcedureAdventure> {
        public Action onShoppingFinish;

        [ShadowFunction]
        protected override void OnEnter(IFsm fsm, IFsmState prev) {
            this.OnEnterShadow(fsm, prev);
            this.onShoppingFinish?.Invoke();
        }

        [ShadowFunction]
        protected override void OnLeave(IFsm fsm, IFsmState next) {
            this.onShoppingFinish = null;
            this.OnLeaveShadow(fsm, next);
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