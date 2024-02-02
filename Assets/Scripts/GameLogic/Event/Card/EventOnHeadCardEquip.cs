using Hsenl.ability;
using Hsenl.actor;
using Hsenl.EventType;

namespace Hsenl {
    public class EventOnHeadCardEquip : AEventSync<OnHeadCardEquip> {
        protected override void Handle(OnHeadCardEquip arg) {
            switch (arg.card.Source) {
                // case AbilityActorConfig abilityConfig: {
                //     var ability = AbilityFactory.CreateActorAbility(abilityConfig);
                //     arg.card.source = ability;
                //     var holder = arg.cardBar.GetHolder().GetComponent<AbilityHolder>();
                //     var trigger = ability.GetComponent<ControlTrigger>();
                //     if (trigger != null) trigger.ControlCode = (int)arg.slot.ControlCode;
                //     holder.EquipAbility(ability);
                //     break;
                // }

                case Ability ability: {
                    var abilityBar = arg.cardBar.AttachedBodied.FindScopeInBodied<AbilityBar>();
                    var trigger = ability.GetComponent<ControlTrigger>();
                    if (trigger != null) {
                        // 如果技能本身的控制码==0, 那就用槽的控制码作为他的控制码
                        if (trigger.ControlCode == 0) {
                            trigger.ControlCode = (int)arg.slot.ControlCode;
                        }
                    }

                    abilityBar.EquipAbility(ability);
                    break;
                }
            }
        }
    }
}