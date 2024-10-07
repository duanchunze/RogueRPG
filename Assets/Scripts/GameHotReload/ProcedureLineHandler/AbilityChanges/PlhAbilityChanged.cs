using System;

namespace Hsenl {
    [ProcedureLineHandlerPriority(0)]
    public class PlhAbilityChanged : AProcedureLineHandler<PliAbilityChangedForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliAbilityChangedForm item, object userToken) {
            return ProcedureLineHandleResult.Success;
        }
    }
}