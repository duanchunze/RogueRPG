namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.MissBeSelectOnHarming)]
    public class PlhHarm_MissBeSelect : AProcedureLineHandler<PliHarmForm, PlwMissBeSelectOnHarming> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, PlwMissBeSelectOnHarming worker, object userToken) {
            // 造成伤害前, 让自己丢失目标
            var selectionTarget = item.harmable?.GetComponent<SelectionTarget>();
            if (selectionTarget == null) {
                return ProcedureLineHandleResult.Success;
            }

            for (int i = selectionTarget.Selectors.Count - 1; i >= 0; i--) {
                var selector = selectionTarget.Selectors[i];
                if (selector.PrimarySelection == selectionTarget) {
                    selector.PrimarySelection = null;
                }
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}