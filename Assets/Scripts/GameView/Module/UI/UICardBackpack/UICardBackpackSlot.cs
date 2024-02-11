using Hsenl.EventType;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    public class UICardBackpackSlot : UICardSlot {
        protected override void OnButtonClick() {
            EventSystem.Publish(new MoveCard() {
                destination = typeof(CardBar),
                cardInstanceId = this.FillerInstanceId,
                slotInstanceId = -1
            });
        }

        protected override void OnEndDrag(PointerEventData data) {
            var slot = UnityHelper.UI.GetComponentInPoint<UICardSlot>(this.GetDragerPosition());
            switch (slot) {
                case UICardBarSlot uiCardBarSlot: {
                    EventSystem.Publish(new MoveCard() {
                        destination = typeof(CardBar),
                        cardInstanceId = this.FillerInstanceId,
                        slotInstanceId = uiCardBarSlot.SlotInstanceId
                    });
                    break;
                }

                case UICardBackpackSlot uiCardBackpackSlot: {
                    EventSystem.Publish(new MoveCard() {
                        destination = typeof(CardBackpack),
                        cardInstanceId = this.FillerInstanceId,
                        slotInstanceId = uiCardBackpackSlot.SlotInstanceId
                    });
                    break;
                }

                case UICardStoreSlot uiCardStoreSlot: {
                    EventSystem.Publish(new MoveCard() {
                        destination = typeof(CardStore),
                        cardInstanceId = this.FillerInstanceId,
                        slotInstanceId = uiCardStoreSlot.SlotInstanceId
                    });
                    break;
                }
            }
        }
    }
}