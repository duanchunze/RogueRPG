namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.TakeHurt)]
    public class PlhHarmTakeHurt : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            PliHurtForm hurtForm = new() {
                harmable = item.harm,
                hurtable = item.hurt,
                damageType = item.damageType,
                deductHp = item.finalDamage,

                iseva = item.iseva,
                isblk = item.isblk,
                backhit = item.backhit,
                ispcrit = item.ispcrit,
                fluctuate = item.fluctuate,
                astun = item.astun,
                hstun = item.hstun,
                score = item.score,
            };

            item.hurt.Bodied.GetComponent<ProcedureLine>().StartLineAsync(hurtForm).Coroutine();
            return ProcedureLineHandleResult.Success;
        }
    }
}