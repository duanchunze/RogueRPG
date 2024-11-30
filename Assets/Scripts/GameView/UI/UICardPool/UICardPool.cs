using UnityEngine;

namespace Hsenl.View {
    // 临时写就, 只供内部测试用
    public class UICardPool: UI<UICardPool> {
        public RectTransform holder;
        public UICardPoolSlot slotTemplate;

        protected override void OnOpen() {
            using var list = ListComponent<Bright.Config.BeanBase>.Rent();
            
            list.AddRange(Tables.Instance.TbAbilityConfig.DataList);
            list.AddRange(Tables.Instance.TbAbilityPatchConfig.DataList);
            list.AddRange(Tables.Instance.TbPropConfig.DataList);
            
            this.holder.MakeSureChildrenCount(this.slotTemplate.transform, list.Count);
            for (int i = 0; i < list.Count; i++) {
                var slot = this.holder.GetChild(i).GetComponent<UICardPoolSlot>();
                var config = list[i];
                slot.FillIn(config);
            }
        }
    }
}