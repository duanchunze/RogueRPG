using System;
using System.Text;
using Hsenl.View;
using Unity.Mathematics;
using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliHurtPriority.ShowUIMessage)]
    public class PlhHurtShowUIMessage : AProcedureLineHandler<PliHurtForm> {
        private StringBuilder _stringBuilder = new();

        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliHurtForm item) {
            var height = item.hurtable.GetComponent<Numerator>().GetValue(NumericType.Height);
            this._stringBuilder.Clear();
            this._stringBuilder.Append(item.deductHp.ToString());
            if (item.backhit) {
                this._stringBuilder.Append(" BC");
            }

            if (item.ispcrit) {
                this._stringBuilder.Append(" BJ");
            }

            switch (item.damageType) {
                case DamageType.PhysicalDamage:
                    UIShortcut.ShowJumpMessage(this._stringBuilder.ToString(),
                        item.hurtable.transform.Position,
                        new float3(0, 0, height + 0.5f),
                        new float3(RandomHelper.mtRandom.NextFloat(-0.1f, 0.1f), 0, RandomHelper.mtRandom.NextFloat(0f, 0.1f)),
                        1,
                        Color.red
                    );

                    break;
                case DamageType.TrueDamage:
                    UIShortcut.ShowJumpMessage(this._stringBuilder.ToString(),
                        item.hurtable.transform.Position,
                        new float3(0, 0, height + 0.5f),
                        new float3(RandomHelper.mtRandom.NextFloat(-0.1f, 0.1f), 0, RandomHelper.mtRandom.NextFloat(0f, 0.1f)),
                        1,
                        Color.white
                    );
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ProcedureLineHandleResult.Success;
        }
    }
}