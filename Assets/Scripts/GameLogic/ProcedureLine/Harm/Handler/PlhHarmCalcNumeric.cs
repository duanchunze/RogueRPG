using System;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliDamageArbitramentPriority.CalcNumeric)]
    public class PlhHarmCalcNumeric : AProcedureLineHandler<PliDamageArbitramentForm> {
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliDamageArbitramentForm item) {
            var harmNumerator = item.HarmNumerator;
            var hurtNumerator = item.HurtNumerator;

            if (item.source is Status status) { }

            // item.astun += harmNumerator.GetValue(NumericType.Astun); // 这里不再次增加了, 该值同damage一样, 交给源头赋值
            item.hstun += hurtNumerator.GetValue(NumericType.Hstun);

            switch (item.damageType) {
                case DamageType.PhysicalDamage:
                    if (harmNumerator != null) {
                        item.dex += harmNumerator.GetValue(NumericType.Dex);
                        item.pcrit += harmNumerator.GetValue(NumericType.Pcirt);
                        item.pcit += harmNumerator.GetValue(NumericType.Pcit);
                        item.pvamp += harmNumerator.GetValue(NumericType.Pvamp);
                    }

                    item.amr += hurtNumerator.GetValue(NumericType.Amr);
                    item.eva += hurtNumerator.GetValue(NumericType.Eva);
                    item.blk += hurtNumerator.GetValue(NumericType.Blk);
                    break;
                case DamageType.TrueDamage:
                    break;
                case DamageType.LightDamage:
                    item.lrt = hurtNumerator.GetValue(NumericType.Lrt);
                    break;
                case DamageType.DarkDamage:
                    item.drt = hurtNumerator.GetValue(NumericType.Drt);
                    break;
                case DamageType.FireDamage:
                    item.frt = hurtNumerator.GetValue(NumericType.Frt);
                    break;
                case DamageType.IceDamage:
                    item.irt = hurtNumerator.GetValue(NumericType.Irt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}