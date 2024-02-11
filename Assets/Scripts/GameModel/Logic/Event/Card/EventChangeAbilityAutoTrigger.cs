using System;
using Hsenl.EventType;

namespace Hsenl {
    public class EventChangeAbilityAutoTrigger : AEventSync<ChangeAbilityAutoTrigger> {
        protected override void Handle(ChangeAbilityAutoTrigger arg) {
            var card = EventSystem.GetInstance<Card>(arg.cardInstanceId);
            if (card == null) throw new Exception($"cant find card with instance id '{arg.cardInstanceId}'");
            var slot = EventSystem.GetInstance<CardBarHeadSlot>(arg.cardBarSlotInstanceId);
            if (slot == null) throw new Exception($"cant find card bar head slot with instance id '{arg.cardBarSlotInstanceId}'");
            var cardBar = slot.FindScopeInParent<CardBar>();
            if (cardBar == null) throw new Exception($"get card bar fail"); 

            if (card.Source is Ability ability) {
                var controlTrigger = ability.GetComponent<ControlTrigger>();
                if (controlTrigger.ControlCode == (int)ControlCode.AutoTrigger) {
                    controlTrigger.ControlCode = (int)slot.ControlCode;
                }
                else {
                    controlTrigger.ControlCode = (int)ControlCode.AutoTrigger;
                }

                cardBar.OnChanged();
            }
        }
    }
}