using System;

namespace Hsenl {
    /// <summary>
    /// 标记在类上: 设置了targetType代表是影子类, 不设置targetType代表是源类.
    /// 标记在函数上: 标记在源类中的函数上代表该函数是源函数, 标记在影子类的函数上代表该函数是影子函数.
    /// 被标记的类需要添加 partial 关键词
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
    public class ShadowFunctionAttribute : BaseAttribute {
        // 注意: 下面声明的字段不可修改
        
        // 由影子端的影子类使用
        public Type TargetType { get; }

        // 由影子端使用, 当一个源函数有多个影子函数时, 用来给每个影子函数排序, 如果在影子类上赋值, 代表该类下所有影子函数都是该priority, 除非该函数自己指定了priority
        public int Priority { get; }

        // 由源函数使用, 是否允许有多个影子函数实现, 如果为false, 那么一个源函数在全域内, 只能有一个对应的影子函数
        public bool AllowMultiShadowFuncs { get; }

        public ShadowFunctionAttribute(Type targetType = null, int priority = 0, bool allowMultiShadowFuncs = false) {
            this.TargetType = targetType;
            this.Priority = priority;
            this.AllowMultiShadowFuncs = allowMultiShadowFuncs;
        }
    }
}