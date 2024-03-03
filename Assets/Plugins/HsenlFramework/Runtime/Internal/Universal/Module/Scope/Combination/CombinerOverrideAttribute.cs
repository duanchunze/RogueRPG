using System;

namespace Hsenl {
    // 覆盖关系: 比如现在有两个组合 ABC 和 AC, 我现在想要一个效果, 就是当有 ABC组合的时候, 会覆盖掉 AC组合, 或者, 当有AC组合的时候, ABC组合就不触发了.
    // 规则:
    // 不能在AC里定义覆盖ABC的同时, 又在ABC里定义覆盖AC, 那样会形成互斥
    // 覆盖组合只能用于MultiConbiner, 或者Multi覆盖Cross, 而不能Cross覆盖Multi
    // 被覆盖的组合会立即断开组合, 且只要它的覆盖者还存在, 它就不会再次形成组合
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinerOverrideAttribute : BaseAttribute {
        public readonly Type[] overrides;
        
        public CombinerOverrideAttribute(params Type[] args) {
            this.overrides = args;
        }
    }
}