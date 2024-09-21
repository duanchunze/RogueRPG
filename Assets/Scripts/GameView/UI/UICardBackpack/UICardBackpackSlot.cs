// using Hsenl.EventType;
// using UnityEngine.EventSystems;
//
// namespace Hsenl.View {
//     public class UICardBackpackSlot : UICardSlot {
//         protected override void OnButtonClick() {
//             CardManager.Instance.TransferCard(this.FillerInstanceId, typeof(CardBar));
//         }
//
//         protected override void OnEndDrag(PointerEventData data) {
//             var slot = UnityHelper.UI.GetComponentInPoint<UICardSlot>(this.GetDragerPosition());
//             switch (slot) {
//                 case UICardBarSlot uiCardBarSlot: {
//                     CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardBarSlot.SlotInstanceId);
//                     break;
//                 }
//
//                 case UICardBackpackSlot uiCardBackpackSlot: {
//                     CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardBackpackSlot.SlotInstanceId);
//                     break;
//                 }
//
//                 case UICardStoreSlot uiCardStoreSlot: {
//                     CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardStoreSlot.SlotInstanceId);
//                     break;
//                 }
//             }
//         }
//     }
// }