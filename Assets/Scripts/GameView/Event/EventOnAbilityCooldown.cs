using Hsenl.EventType;

namespace Hsenl.View {
    public class EventOnAbilityCooldown : AEventSync<OnAbilityCooldown> {
        protected override void Handle(OnAbilityCooldown arg) {
            if (arg.ability.GetHolder() != GameManager.Instance.MainMan) return;

            var bar = UICardBar.instance;
            if (bar == null) return;

            var card = arg.ability.GetLinker<Card>();
            if (card == null) return;

            var slot = (UICardBarHeadSlot)bar.GetSlotOfCard(card);
            slot.RunCooldown(arg.cooltime, arg.cooltilltime);
        }
    }
}