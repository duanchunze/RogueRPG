namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.RecoverEnergy)]
    public class PlhHarm_RecoverEnergy : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            switch (item.source) {
                case Ability abi:
                    

                    break;
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}