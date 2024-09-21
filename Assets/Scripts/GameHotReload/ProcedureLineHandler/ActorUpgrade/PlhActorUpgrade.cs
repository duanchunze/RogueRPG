namespace Hsenl {
    [ProcedureLineHandlerPriority(PliActorUpgradePriority.DrawCard)]
    public class PlhActorUpgrade : AProcedureLineHandlerAsync<PliActorUpgradeForm> {
        protected override async HTask<ProcedureLineHandleResult> Handle(ProcedureLine procedureLine, PliActorUpgradeForm item) {
            var pl = item.actor.GetComponent<ProcedureLine>();
            await pl.StartLineAsync(new PliDrawCardForm() {
                target = item.actor,
            });
            return ProcedureLineHandleResult.Success;
        }
    }
}