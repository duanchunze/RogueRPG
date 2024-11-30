namespace Hsenl {
    [ProcedureLineHandlerPriority(PliResurgencePriority.NoramlHandle)]
    public class PlhResurgence_NormalHandle : AProcedureLineHandler<PliResurgenceForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliResurgenceForm item, object userToken) {
            var agent = item.target.transform.NavMeshAgent;
            if (agent != null) {
                agent.Enable = true;
            }

            if (item.target is Actor actor) {
                var numerator = actor.GetComponent<Numerator>();
                Shortcut.RecoverHealth(numerator, int.MaxValue);
                Shortcut.RecoverMana(numerator, int.MaxValue);
            }

            var abiBar = item.target.FindBodiedInIndividual<AbilitesBar>();
            if (abiBar != null) {
                Shortcut.ResetAllAbilitiesCD(abiBar);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}