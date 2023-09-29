using System.Collections.Generic;

namespace Hsenl.card {
    public partial class TbCardConfig {
        public card.CardConfig[] GetOfType<T>() where T : card.Info {
            using var list = ListComponent<card.CardConfig>.Create();
            foreach (var value in this._dataList) {
                if (value.Wrappage is T t)
                    list.Add(value);
            }

            return list.ToArray();
        }
    }
}