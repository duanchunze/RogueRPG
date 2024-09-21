using System;
using System.Text;
using Hsenl.View;
using UnityEngine;

namespace Hsenl {
    [ProcedureLineHandlerPriority(PliRecoverPriority.ViewExpression)]
    public class PlhRecover_ShowUIMessage : AProcedureLineHandler<PliRecoverForm> {
        private StringBuilder _stringBuilder = new();
        
        protected override ProcedureLineHandleResult Handle(ProcedureLine procedureLine, ref PliRecoverForm item) {
            var height = item.target.GetComponent<Numerator>().GetValue(NumericType.Height);
            this._stringBuilder.Clear();
            this._stringBuilder.Append("+ ");
            this._stringBuilder.Append(item.recoverHp);

            ViewShortcut.ShowJumpMessage(
                this._stringBuilder.ToString(),
                item.target.transform.Position,
                new Vector3(0, height + 0.2f, 0),
                new Vector3(RandomHelper.NextFloat(-0.1f, 0.1f), 0, RandomHelper.NextFloat(0f, 0.1f)),
                Vector3.One,
                Color.green
            );
            return ProcedureLineHandleResult.Success;
        }
    }
}