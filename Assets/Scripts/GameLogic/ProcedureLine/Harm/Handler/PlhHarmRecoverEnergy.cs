namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.RecoverEnergy)]
    public class PlhHarmRecoverEnergy : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            switch (item.source) {
                case Ability abi:
                    

                    break;
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}