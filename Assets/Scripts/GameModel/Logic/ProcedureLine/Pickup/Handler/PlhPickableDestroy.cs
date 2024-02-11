namespace Hsenl.Handler {
    [ProcedureLineHandlerPriority(PliPickupPriority.Destroy)]
    public class PlhPickableDestroy : AProcedureLineHandler<PliPickupForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliPickupForm item) {
            PickableManager.Instance.Return(item.pickable);
            return ProcedureLineHandleResult.Success;
        }
    }
}