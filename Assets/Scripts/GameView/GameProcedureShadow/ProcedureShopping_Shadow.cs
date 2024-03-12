using System;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureShopping))]
    public static partial class ProcedureShopping_Shadow {
        [ShadowFunction]
        private static void OnEnter(ProcedureShopping self, ProcedureManager manager, FsmState<ProcedureManager> prev) {
            UIManager.SingleOpen<UICardStore>(UILayer.High);
            var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            bar.ShowAbilityAssist();
            UIManager.SingleOpen<UICardBackpack>(UILayer.High);
            var uishopping = UIManager.SingleOpen<UIShopping>(UILayer.High);
            uishopping.onShoppingFinish += self.OnShoppingFinish;

            CardStore.Instance.refreshFreeTime++;
            GameManager.Instance.SetGold(Tables.Instance.TbGameSingletonConfig.CoinEach);
            GameManager.Instance.ProcedureLine.StartLine(new PliRefreshStoreCardsForm());
        }

        [ShadowFunction]
        private static void OnLeave(ProcedureShopping self, ProcedureManager manager, FsmState<ProcedureManager> next) {
            UIManager.SingleClose<UICardStore>();
            UIManager.SingleClose<UICardBar>();
            UIManager.SingleClose<UICardBackpack>();
            var uishopping = UIManager.SingleClose<UIShopping>();
            uishopping.onShoppingFinish -= self.OnShoppingFinish;
        }
    }
}