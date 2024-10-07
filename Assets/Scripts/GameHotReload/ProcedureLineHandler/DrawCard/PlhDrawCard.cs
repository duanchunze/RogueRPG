namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDrawCardPriority.RefreshCards)]
    public class PlhDrawCard : AProcedureLineHandlerAsync<PliDrawCardForm> {
        protected override async HTask<ProcedureLineHandleResult> Handle(ProcedureLine procedureLine, PliDrawCardForm item, object userToken) {
            var drawcard = item.target.FindBodiedInIndividual<DrawCard>();
            await drawcard.StartDrawCard();
            return ProcedureLineHandleResult.Success;
        }
    }
}