namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDiePriority.DestoryDeadBody)]
    public class PlhDie_DestoryDeadBody : AProcedureLineHandler<PliDieForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDieForm item, object userToken) {
            if (item.target is Actor actor) {
                if (!Shortcut.IsMainMan(actor)) {
                    this.WaitForDestoryBody(actor);
                }
            }

            return ProcedureLineHandleResult.Success;
        }

        private async void WaitForDestoryBody(Actor actor) {
            await Timer.WaitTime(3000);
            ActorManager.Instance.Return(actor);
        }
    }
}