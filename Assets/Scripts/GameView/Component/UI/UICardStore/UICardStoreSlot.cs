using Hsenl.EventType;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    public class UICardStoreSlot : UICardSlot {
        protected override void OnButtonClick() {
            CardManager.Instance.TransferCard(this.FillerInstanceId, typeof(CardBar));
        }

        protected override void OnRightButtonClick() {
            CardManager.Instance.FreezeCard(this.FillerInstanceId);
        }

        protected override void OnEndDrag(PointerEventData data) {
            var slot = UnityHelper.UI.GetComponentInPoint<UICardSlot>(this.GetDragerPosition());
            switch (slot) {
                case UICardBarSlot uiCardBarSlot: {
                    CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardBarSlot.SlotInstanceId);
                    break;
                }

                case UICardBackpackSlot uiCardBackpackSlot: {
                    CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardBackpackSlot.SlotInstanceId);
                    break;
                }
            }
        }
    }
}