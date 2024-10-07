namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDiePriority.DropItem)]
    public class PlhDie_DropItem : AProcedureLineHandler<PliDieForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDieForm item, object userToken) {
            item.target.GetComponent<Dropable>()?.Drop(DropMode.FixedCountNotRepeat);
            return ProcedureLineHandleResult.Success;
        }
    }
}