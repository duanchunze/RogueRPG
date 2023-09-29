using Hsenl.EventType;

namespace Hsenl {
    public class EventFreezeCard : AEventSync<FreezeCard> {
        protected override void Handle(FreezeCard arg) {
            var card = EventSystem.GetInstance<Card>(arg.cardInstanceId);
            if (card == null)
                return;
            
            if (card.StaySlot is not CardStoreSlot cardStoreSlot)
                return;

            cardStoreSlot.freeze = !cardStoreSlot.freeze;
            var cardStore = cardStoreSlot.GetParentSubstantiveAs<CardStore>();
            cardStore.OnChanged();
        }
    }
}