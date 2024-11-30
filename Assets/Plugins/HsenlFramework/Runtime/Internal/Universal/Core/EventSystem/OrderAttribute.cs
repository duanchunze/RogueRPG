using System;

namespace Hsenl {
    // 越小越先触发. 目前支持: Event、MethodEvent.
    [AttributeUsage(AttributeTargets.All)]
    public class OrderAttribute : BaseAttribute {
        public readonly int order;

        public OrderAttribute(int order) {
            this.order = order;
        }
    }
}