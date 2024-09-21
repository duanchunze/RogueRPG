namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHarmPriority.TakeHurt)]
    public class PlhHarm_TakeHurt : AProcedureLineHandler<PliHarmForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHarmForm item) {
            PliHurtForm hurtForm = new() {
                harmable = item.harmable,
                hurtable = item.hurtable,
                damageType = item.damageType,
                deductHp = item.finalDamage,

                iseva = item.iseva,
                isblk = item.isblk,
                sneakAtk = item.sneakAtk,
                backhit = item.backhit,
                ispcrit = item.ispcrit,
                fluctuate = item.fluctuate,
                astun = item.astun,
                hstun = item.hstun,
                score = item.score,
            };

            item.hurtable.Bodied.GetComponent<ProcedureLine>().StartLineAsync(hurtForm).Tail();
            return ProcedureLineHandleResult.Success;
        }
    }
}