using Hsenl.EventType;
using UnityEngine.EventSystems;

namespace Hsenl.View {
    public class UICardStorePoolSlot : UICardSlot {
        protected override void OnButtonClick() {
            CardManager.Instance.TransferCard(this.FillerInstanceId, typeof(CardBar), true);
        }
    }
}