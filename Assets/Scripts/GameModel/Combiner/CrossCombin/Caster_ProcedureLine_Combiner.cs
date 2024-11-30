using System;

namespace Hsenl.CrossCombiner {
    public class Caster_ProcedureLine_Combiner : CrossCombiner<Caster, ProcedureLine> {
        protected override void OnCombin(Caster arg1, ProcedureLine arg2) {
            arg1.onCastEnd += this.EnqueueAction<Action<CasterEndDetails>>(details => {
                switch (details.endType) {
                    case CasterEndType.Complated:
                        break;
                    case CasterEndType.InitiativeInvoke:
                        break;
                    case CasterEndType.Exclusion:
                        break;
                    case CasterEndType.Break:
                        // 交给流水线处理
                        var form = new PliCasterBreakForm {
                            caster = arg1,
                            interceptor = details.initiator,
                        };

                        arg2.StartLine(ref form);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
        }

        protected override void OnDecombin(Caster arg1, ProcedureLine arg2) {
            arg1.onCastEnd -= this.DequeueAction<Action<CasterEndDetails>>();
        }
    }
}