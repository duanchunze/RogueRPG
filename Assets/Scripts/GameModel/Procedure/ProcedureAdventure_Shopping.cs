using System;

namespace Hsenl {
    [ShadowFunction]
    public partial class ProcedureAdventure_Shopping : AProcedureState<ProcedureAdventure> {
        public Action onShoppingFinish;

        [ShadowFunction]
        protected override HTask OnEnter(IFsm fsm, IFsmState prev) {
            this.OnEnterShadow(fsm, prev);
            this.onShoppingFinish?.Invoke();
            return default;
        }

        [ShadowFunction]
        protected override HTask OnLeave(IFsm fsm, IFsmState next) {
            this.onShoppingFinish = null;
            this.OnLeaveShadow(fsm, next);
            return default;
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