using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliPickupPriority.Pickup)]
    public class PlhPickup : AProcedureLineHandler<PliPickupForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliPickupForm item, object userToken) {
            var config = item.pickable.Config;
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