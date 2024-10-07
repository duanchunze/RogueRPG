namespace Hsenl {
    [ProcedureLineHandlerPriority(PliPickupPriority.Destroy)]
    public class PlhPickup_PickableDestroy : AProcedureLineHandler<PliPickupForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliPickupForm item, object userToken) {
            PickableManager.Instance.Return(item.pickable);
            return ProcedureLineHandleResult.Success;
        }
    }
}