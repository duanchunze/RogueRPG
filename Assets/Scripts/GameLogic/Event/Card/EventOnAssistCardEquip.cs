using System;
using Hsenl.EventType;

namespace Hsenl {
    public class EventOnAssistCardEquip : AEventSync<OnAssistCardEquip> {
        protected override void Handle(OnAssistCardEquip arg) {
            var headCardSlot = arg.slot.FindScopeInParent<CardBarHeadSlot>();
            var headCard = headCardSlot.StayCard;
            if (headCard == null) 
                throw new Exception("head card not exist");
            
            switch (arg.card.Source) {
                case AbilityAssist abilityAssist: {
                    abilityAssist.SetParent(headCard.Source.Entity);
                    abilityAssist.transform.NormalTransfrom();
                    break;
                }
            }
        }
    }
}