using System;
using Hsenl.View;

namespace Hsenl {
    public class ProcedureShopping : AProcedureState {
        public Action onShoppingFinish;

        protected override void OnEnter(ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UICardStore>(UILayer.High);
            var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            bar.ShowAbilityAssist();
            UIManager.SingleOpen<UICardBackpack>(UILayer.High);
            var uishopping = UIManager.SingleOpen<UIShopping>(UILayer.High);
            uishopping.onShoppingFinish += this.OnShoppingFinish;

            CardStore.Instance.refreshFreeTime++;
            GameManager.Instance.SetGold(Tables.Instance.TbGameSingletonConfig.CoinEach);
            GameManager.Instance.ProcedureLine.StartLine(new PliRefreshStoreCardsForm());
        }

        protected override void OnLeave(ProcedureManager manager, FsmState<ProcedureManager> next) {
            this.onShoppingFinish = null;

            UIManager.SingleClose<UICardStore>();
            UIManager.SingleClose<UICardBar>();
            UIManager.SingleClose<UICardBackpack>();
            var uishopping = UIManager.SingleClose<UIShopping>();
            uishopping.onShoppingFinish -= this.OnShoppingFinish;
        }

        private void OnShoppingFinish() {
            try {
                this.onShoppingFinish?.Invoke();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }
    }
}