namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDiePriority.DropExp)]
    public class PlhDie_DropExp : AProcedureLineHandler<PliDieForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDieForm item, object userToken) {
            // 获取掉落的经验值, 添加给施加者, 也是通过pl方式
            var dropable = item.target.GetComponent<Dropable>();
            var pl = item.inflictor.GetComponent<ProcedureLine>();
            pl.StartLineAsync(new PliGainExpForm() {
                target = item.inflictor,
                exp = dropable.killExp,
            }).Tail();
            return ProcedureLineHandleResult.Success;
        }
    }
}