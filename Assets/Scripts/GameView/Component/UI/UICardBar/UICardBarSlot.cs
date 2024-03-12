using System;
using Hsenl.EventType;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardBarSlot : UICardSlot {
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

                case UICardStoreSlot uiCardStoreSlot: {
                    CardManager.Instance.TransferCard(this.FillerInstanceId, uiCardStoreSlot.SlotInstanceId);
                    break;
                }
            }
        }
    }
}