using System;

namespace Hsenl {
    /// <summary>
    /// 只针对 start update lateUpdate 几个事件的执行顺序
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExecutionOrderAttribute : BaseAttribute {
        public readonly int order;

        public ExecutionOrderAttribute(int order) {
            this.order = order;
        }
    }
}