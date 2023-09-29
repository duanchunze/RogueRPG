using System;

namespace Hsenl.CrossCombiner {
    public class CasterProcedureLineCombiner : CrossCombiner<Caster, ProcedureLine> {
        protected override void OnCombin(Caster arg1, ProcedureLine arg2) {
            arg1.onIntercepted += this.EnqueueAction<Action<Object>>(caster => {
                // 交给流水线处理
            });
            arg1.onBreak += this.EnqueueAction<Action<Object>>(caster => {
                // 交给流水线处理
                var form = new PliCasterBreakForm {
                    caster = arg1,
                    interceptor = caster,
                };

                arg2.StartLine(ref form);
            });
        }

        protected override void OnDecombin(Caster arg1, ProcedureLine arg2) {
            arg1.onIntercepted -= this.DequeueAction<Action<Object>>();
            arg1.onBreak -= this.DequeueAction<Action<Object>>();
        }
    }
}