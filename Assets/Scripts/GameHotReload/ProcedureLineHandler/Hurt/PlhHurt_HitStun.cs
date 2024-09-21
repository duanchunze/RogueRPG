namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHurtPriority.HitStun)]
    public class PlhHurt_HitStun : AProcedureLineHandler<PliHurtForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHurtForm item) {
            // 根据攻击硬直与受击硬直的插值, 来给自己上受击状态, 同时, 差值越大, 受击状态持续的时间也越长
            var stunDiff = item.astun - item.hstun;
            if (stunDiff > 0) {
                var duration = GameFormula.HitStun2StatusDuration(stunDiff);
                var status = Shortcut.InflictionStatus(item.harmable.Bodied, item.hurtable.Bodied, StatusAlias.HitStun, duration);
                item.frameSlower = !status.IsEnter;
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}