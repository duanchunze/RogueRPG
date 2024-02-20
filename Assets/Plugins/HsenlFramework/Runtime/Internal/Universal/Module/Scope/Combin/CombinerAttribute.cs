using System;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class CombinerAttribute : BaseAttribute {
        public readonly CombinerType combinerType;

        public CombinerAttribute(CombinerType combinerType) {
            this.combinerType = combinerType;
        }
    }
}