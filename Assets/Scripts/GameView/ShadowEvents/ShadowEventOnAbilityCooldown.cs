namespace Hsenl.View {
    [ShadowFunction(typeof(EventStation))]
    public partial class ShadowEventOnAbilityCooldown {
        [ShadowFunction]
        private static void OnAbilityCooldown(Hsenl.Ability ability, float cooltime, float tilltime) {
            // if (ability.MainBodied != GameManager.Instance.MainMan) return;
            //
            // var bar = UIManager.GetSingleUI<UICardBar>();
            // if (bar == null) return;
            //
            // var card = ability.GetLinker<Card>();
            // if (card == null) return;
            //
            // var slot = (UICardBarHeadSlot)bar.GetSlotOfCard(card);
            // slot.RunCooldown(cooltime, tilltime);
        }
    }
}