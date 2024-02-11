using Hsenl.EventType;

namespace Hsenl {
    public class EventOnAssistCardUnequip : AEventSync<OnAssistCardUnequip> {
        protected override void Handle(OnAssistCardUnequip arg) {
            switch (arg.card.Source) {
                case AbilityAssist abilityAssist: {
                    abilityAssist.SetParent(arg.slot.Entity);
                    abilityAssist.transform.NormalTransfrom();
                    break;
                }
            }
        }
    }
}