using Hsenl.actor;
using Hsenl.EventType;

namespace Hsenl {
    public class EventOnHeadCardUnequip : AEventSync<OnHeadCardUnequip> {
        protected override void Handle(OnHeadCardUnequip arg) {
            switch (arg.card.Source) {
                case Ability ability: {
                    // var trigger = ability.GetComponent<ControlTrigger>();
                    // if (trigger != null) trigger.ControlCode = 0;
                    var holder = GameManager.Instance.MainMan.FindScopeInBodied<AbilityBar>();
                    holder.UnequipAbility(ability);
                    ability.GetLinker<Card>().Reset();
                    break;
                }
            }
        }
    }
}