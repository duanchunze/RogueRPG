namespace Hsenl {
    [ProcedureLineHandlerPriority(PliGainExpPriority.AddNumeric)]
    public class PlhGainExp : AProcedureLineHandlerAsync<PliGainExpForm> {
        protected override async HTask<ProcedureLineHandleResult> Handle(ProcedureLine procedureLine, PliGainExpForm item, object userToken) {
            if (item.exp == 0)
                return ProcedureLineHandleResult.Break;

            var target = item.target;
            var minion = target.GetComponent<Minion>();
            if (minion != null) {
                target = minion.master; // 如果是个随从, 那经验就加给自己的主人
            }

            var numerator = target.GetComponent<Numerator>();
            if (!numerator.IsHasValue(NumericType.MaxExp))
                return ProcedureLineHandleResult.Break;

            var addexp = (long)item.exp;
            var v = Shortcut.AddExp(numerator, addexp);
            var pl = target.GetComponent<ProcedureLine>();
            while (v < addexp && v != -1) {
                // 说明没加完, 升一级, 把经验清零, 提升最大经验值, 再重新加
                Shortcut.AddLv(numerator, 1);
                switch (pl.Bodied) {
                    case Actor actor: {
                        await pl.StartLineAsync(new PliActorUpgradeForm() {
                            actor = actor,
                            upgradeValue = 1,
                        });
                        break;
                    }
                }

                numerator.SetValue(NumericType.Exp, 0);
                addexp = item.exp - v;
                v = Shortcut.AddExp(numerator, addexp);
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}