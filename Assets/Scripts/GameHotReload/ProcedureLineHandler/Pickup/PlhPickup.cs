using UnityEngine;

namespace Hsenl.Handler {
    [ProcedureLineHandlerPriority(PliPickupPriority.Pickup)]
    public class PlhPickup : AProcedureLineHandler<PliPickupForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliPickupForm item) {
            var config = Tables.Instance.TbPickableConfig.GetById(item.pickable.configId);
            switch (config.Wrappage) {
                case pickable.WrappageCoinInfo wrappageCoinInfo: {
                    // 拾取了金币
                    GameManager.Instance.AddGold(wrappageCoinInfo.Count);
                    break;
                }
            }
            
            return ProcedureLineHandleResult.Success;
        }
    }
}