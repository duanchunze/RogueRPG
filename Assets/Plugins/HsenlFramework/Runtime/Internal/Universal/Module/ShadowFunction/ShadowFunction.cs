using System;
using System.Collections.Generic;

namespace Hsenl {
    /// <summary>
    /// <para>影子系统分为两个部分, "源"和"影子", 影子函数相当于是源函数的外部实现.
    /// 二者不能在同一程序集内, 否则不起作用.
    /// 不支持out in, 不支持泛型函数, 支持async, 支持泛型类(但仅支持class<>这种写法, 这是编译器决定的, 后续可以考虑增加使用字符串的形式指定源函数, 不过那样, 就无法通过F12快速跳转了),
    /// 支持显式接口实现的方法, 源类和影子类都分别支持静态和非静态, 支持struct</para>
    ///
    /// <para>用途: 主要用于热重载, 让函数与数据分离, 也可以一定程度上平替Event系统, 影子系统虽然支持分发, 但需要确保分发的影子优先级不重复, 因为影子函数相当于是源函数的外部实现,
    /// 对于函数来说, 如果没有明确的执行顺序的话, 可能会出现未知的错误</para>
    ///
    /// <para>为什么选择影子系统: 最主要的原因就是直观, 便捷, 性能高. 不像使用回调那么调来调去的人都调晕了, 也不像Event系统写起来那么麻烦, 而且性能比前两者都好. 但不是说
    /// 就完全的可以代替前两者, 比如他无法支持自定义的影子函数, 也不能给影子函数分组, 实现自定义调用方案, 这在一定程度上没有event系统灵活,
    /// 当然, 这也主要与影子系统的设计理念有关, 他就是为了做热重载而诞生的.
    /// </para>
    /// 
    /// <para>使用方法: 分两个程序集, 同一个程序集内无法使用. (同一个程序集没理由使用影子系统)
    /// 程序集1 -> class1(添加[ShadowFunctionAttribute]特性) -> Function1(添加[ShadowFunctionAttribute]特性)
    /// 程序集2 -> class1shadow(添加[ShadowFunctionAttribute(typeof(程序集1.class1))]特性) -> Function1(添加[ShadowFunctionAttribute]特性).
    /// 影子函数与源函数的名字, 参数的类型、顺序、数量, 以及返回值类型都必须相同
    /// </para>
    ///
    /// <para>特殊情况: 1、影子函数第一位参数写的如果是源类, 会自动忽略第一位参数进行匹配 2、当源函数是async void这种写法的时候, 影子函数需要写成async ETTask才能正常匹配</para>
    ///
    /// <para>影子系统并不依赖框架本身, 所以即便是和框架无关的类也可以使用, 或者单独拷贝该系统到其他项目里, 也可以正常使用, 只需要自行进行注册就行了. 不仅该系统, 框架的许多模块都不依赖
    /// 于Hsenl框架, 不过有些需要做点小修改.</para>
    /// </summary>
    /*
     * 问题: 针对同一个源函数, 同一个域内, priority不能重复, 但不同的域之间, priority却可以重复
     */
    public static class ShadowFunction {
        public static void Register<T>(uint hashcode, string assemblyName, int priority, T del) where T : Delegate => ShadowFunctionManager.Instance.Register(hashcode, assemblyName, priority, del);
        public static void Unregister(uint hashcode) => ShadowFunctionManager.Instance.Unregister(hashcode);
        public static void Unregister(uint hashcode, string source, int priority) => ShadowFunctionManager.Instance.Unregister(hashcode, source, priority);
        public static bool GetFunctions(uint hashcode, out SortedDictionary<long, Delegate> dels) => ShadowFunctionManager.Instance.GetFunctions(hashcode, out dels);
    }
}