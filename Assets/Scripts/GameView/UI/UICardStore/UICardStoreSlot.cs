﻿using System;
using Hsenl.EventType;
using Hsenl.InvokeType;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Hsenl.View {
    public class UICardStoreSlot : UICardSlot {
        protected override void OnButtonClick() {
            EventSystem.Publish(new MoveCard() {
                destination = typeof(CardBar),
                cardInstanceId = this.FillerInstanceId, 
                slotInstanceId = -1
            });
            
        }

        protected override void OnRightButtonClick() {
            EventSystem.Publish(new FreezeCard() {
                cardInstanceId = this.FillerInstanceId,
            });
        }

        protected override void OnEndDrag(PointerEventData data) {
            var slot = UnityHelper.UI.GetComponentInPoint<UICardSlot>(this.GetDragerPosition());
            switch (slot) {
                case UICardBarSlot uiCardBarSlot: {
                    EventSystem.Publish(new MoveCard {
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
            }
        }
    }
}