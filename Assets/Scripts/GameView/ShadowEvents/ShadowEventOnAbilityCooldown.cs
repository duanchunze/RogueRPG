namespace Hsenl.View {
    [ShadowFunction(typeof(SourceEventStation))]
    public partial class ShadowEventOnAbilityCooldown {
        [ShadowFunction]
        private static void OnAbilityCooldown(Hsenl.Ability ability, float cooltime, float tilltime) {
            if (ability.AttachedBodied != GameManager.Instance.MainMan) return;

            var bar = UICardBar.instance;
            if (bar == null) return;

            var card = ability.GetLinker<Card>();
            if (card == null) return;

            var slot = (UICardBarHeadSlot)bar.GetSlotOfCard(card);
            slot.RunCooldown(cooltime, tilltime);
        }
    }
}