using System;
using System.Collections.Generic;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentAttribute : BaseAttribute {
        public readonly Type requireType;
        public readonly Type addType;

        public RequireComponentAttribute(Type requireType) {
            this.requireType = requireType;
            this.addType = requireType;
        }
        
        public RequireComponentAttribute(Type requireType, Type addType) {
            this.requireType = requireType;
            this.addType = addType;
        }
    }
}