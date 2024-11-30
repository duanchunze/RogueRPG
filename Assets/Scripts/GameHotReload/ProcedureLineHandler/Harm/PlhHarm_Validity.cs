namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.Validity)]
    public class PlhHarm_Validity : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item, object userToken) {
            item.harmNumerator = item.harmNumerator ??= item.harmable?.GetComponent<Numerator>();
            item.hurtNumerator = item.hurtNumerator ??= item.hurtable.GetComponent<Numerator>(); // hurtable是必须要有的, numerator也是必须要有的
            item.sourceNumerator = item.sourceNumerator ??= item.source?.GetComponent<Numerator>();

            // 判断是否有hurt
            {
                if (item.hurtNumerator == null) {
                    Log.Error("harm miss hurtable");
                    return ProcedureLineHandleResult.Break;
                }
            }

            if (Shortcut.IsDead(item.hurtNumerator.Bodied))
                return ProcedureLineHandleResult.Break;

            // 判断伤害类型是否指定
            {
                if (item.damageFormulaInfo != null) {
                    item.damageType = item.damageFormulaInfo.DamageType;
                }

                if (item.damageType == DamageType.None) {
                    Log.Error("damage type is none");
                    return ProcedureLineHandleResult.Break; // 没指定伤害类型, 就不会造成伤害
                }
            }

            { // 初始化伤害比例
                if (item.damageRatio == 0f)
                    item.damageRatio = 1f;
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}