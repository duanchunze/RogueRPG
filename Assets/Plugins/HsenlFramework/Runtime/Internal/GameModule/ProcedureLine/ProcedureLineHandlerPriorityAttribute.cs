using System;

namespace Hsenl {
    // 每个处理者都必须挂载该属性，且同一流水线下，优先级不能重复, 越小的越先执行
    // 顺带一提, 有需要顺序执行的系统, 统一都是越小的先执行 (不包括优先级系统, 当然优先级系统的作用也并不是用来按顺序执行什么什么)
    [AttributeUsage(AttributeTargets.Class)]
    public class ProcedureLineHandlerPriorityAttribute : Attribute {
        public readonly int priority;

        public ProcedureLineHandlerPriorityAttribute(int priority) {
            this.priority = priority;
        }

        public ProcedureLineHandlerPriorityAttribute(object priority) {
            this.priority = (int)priority;
        }
    }
}