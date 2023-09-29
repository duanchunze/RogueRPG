using Hsenl.EventType;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    public class UICardStorePoolSlot : UICardSlot {
        protected override void OnButtonClick() {
            EventSystem.Publish(new MoveCard() {
                destination = typeof(CardBar),
                cardInstanceId = this.FillerInstanceId, 
                slotInstanceId = -1, 
                copyNew = true,
            });
        }
    }
}