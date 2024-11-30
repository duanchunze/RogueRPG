using UnityEngine;

namespace Hsenl.View {
    public class UIActorWarehouse : UI<UIActorWarehouse> {
        public RectTransform holder;
        public UIActorWarehouseSlot slotTemplate;

        protected override void OnOpen() {
            using var list = ListComponent<actor.ActorConfig>.Rent();

            list.AddRange(Tables.Instance.TbActorConfig.DataList);

            this.holder.MakeSureChildrenCount(this.slotTemplate.transform, list.Count);
            for (int i = 0; i < list.Count; i++) {
                var slot = this.holder.GetChild(i).GetComponent<UIActorWarehouseSlot>();
                var config = list[i];
                slot.FillIn(config);
            }
        }
    }
}