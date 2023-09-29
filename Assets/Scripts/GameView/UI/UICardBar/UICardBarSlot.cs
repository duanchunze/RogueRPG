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
                    EventSystem.Publish(new MoveCard() {
                        destination = typeof(CardBar),
                        cardInstanceId = this.FillerInstanceId, 
                        slotInstanceId = uiCardBarSlot.SlotInstanceId
                    });
                    break;
                }

                case UICardBackpackSlot uiCardBackpackSlot: {
                    EventSystem.Publish(new MoveCard {
                        destination = typeof(CardBackpack),
                        cardInstanceId = this.FillerInstanceId, 
                        slotInstanceId = uiCardBackpackSlot.SlotInstanceId
                    });
                    break;
                }

                case UICardStoreSlot uiCardStoreSlot: {
                    EventSystem.Publish(new MoveCard {
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