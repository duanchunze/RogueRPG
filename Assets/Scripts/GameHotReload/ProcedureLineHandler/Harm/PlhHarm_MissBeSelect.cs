namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.MissBeSelectOnHarming)]
    public class PlhHarm_MissBeSelect : AProcedureLineHandler<PliHarmForm, PlwMissBeSelectOnHarming> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, PlwMissBeSelectOnHarming worker,
            object userToken) {
            // 造成伤害前, 让自己丢失目标
            var selectionTarget = item.harmable?.GetComponent<SelectionTargetDefault>();
            if (selectionTarget == null) {
                return ProcedureLineHandleResult.Success;
            }

            selectionTarget.ClearAllSelectors();

            return ProcedureLineHandleResult.Success;
        }
    }
}