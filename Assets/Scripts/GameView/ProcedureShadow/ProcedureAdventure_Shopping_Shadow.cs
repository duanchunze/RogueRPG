using System;

namespace Hsenl.View {
    [ShadowFunction(typeof(ProcedureAdventure_Shopping))]
    public static partial class ProcedureAdventure_Shopping_Shadow {
        [ShadowFunction]
        private static HTask OnEnter(ProcedureAdventure_Shopping self, IFsm fsm, IFsmState prev) {
            // UIManager.SingleOpen<UICardStore>(UILayer.High);
            // var bar = UIManager.SingleOpen<UICardBar>(UILayer.High);
            // bar.ShowAbilityAssist();
            // UIManager.SingleOpen<UICardBackpack>(UILayer.High);
            // var uishopping = UIManager.SingleOpen<UIShopping>(UILayer.High);
            // uishopping.onShoppingFinish += self.OnShoppingFinish;
            //
            // CardStore.Instance.refreshFreeTime++;
            // GameManager.Instance.SetGold(Tables.Instance.TbGameSingletonConfig.CoinEach);
            // GameManager.Instance.ProcedureLine.StartLine(new PliRefreshStoreCardsForm());
            return default;
        }

        [ShadowFunction]
        private static HTask OnLeave(ProcedureAdventure_Shopping self, IFsm fsm, IFsmState next) {
            // UIManager.SingleClose<UICardStore>();
            // // UIManager.SingleClose<UICardBar>();
            // var bar = UIManager.GetSingleUI<UICardBar>();
            // bar.HideAbilityAssist();
            // UIManager.SingleClose<UICardBackpack>();
            // var uishopping = UIManager.SingleClose<UIShopping>();
            // uishopping.onShoppingFinish -= self.OnShoppingFinish;
            return default;
        }
    }
}