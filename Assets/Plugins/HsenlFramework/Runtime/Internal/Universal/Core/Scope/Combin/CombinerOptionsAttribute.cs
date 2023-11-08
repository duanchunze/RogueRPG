using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinerOptionsAttribute : BaseAttribute {
        public int crossSplitPosition = 1; // 该值定义了一个拆分位置, 该位置前为主类型, 该位置以及其后面, 为次类型
        public int crossMaximumLayer = 1;
    }
}