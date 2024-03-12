namespace Hsenl {
    [ProcedureLineHandlerPriority(PliStatusChangesPriority.Drop)]
    public class PlhDeathDrop : AProcedureLineHandler<PliStatusChangesForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliStatusChangesForm item) {
            if (item.changeType == 0) {
                switch (item.statusAlias) {
                    case StatusAlias.Death: {
                        item.target.GetComponent<Dropable>()?.Drop(DropMode.FixedCountNotRepeat);

                        break;
                    }
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}