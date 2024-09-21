using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinerOptionsAttribute : BaseAttribute {
        public int crossSplitPosition = 1; // 该值定义了一个拆分位置, 该位置前为主类型, 该位置以及其后面, 为次类型
        public int crossMaximumLayer = 1; // 当前组合被允许的最大层数匹配层数. 默认为1层. 如果是手动做的匹配或者是ScopeCombinFormatter指定的匹配, 则不受该值限制
    }
}