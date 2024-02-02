using System;
using System.Collections.Generic;

namespace Hsenl {
    [AttributeUsage(AttributeTargets.Class)]
    public class RequireComponentAttribute : BaseAttribute {
        public readonly Type requireType; // 需求的组件, 支持多态
        public readonly Type addType; // 当没有找到需求组件时, 添加的组件

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