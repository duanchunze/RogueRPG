using System;

namespace Hsenl {
    /// <summary>
    /// 标记在类上: 设置了targetType代表是影子类, 不设置targetType代表是源类.
    /// 标记在函数上: 标记在源类中的函数上代表该函数是源函数, 标记在影子类的函数上代表该函数是影子函数.
    /// 被标记的类需要添加 partial 关键词
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public partial class ShadowFunctionAttribute : BaseAttribute {
        public Type targetType;

        public ShadowFunctionAttribute(Type targetType = null) {
            this.targetType = targetType;
        }
    }
}