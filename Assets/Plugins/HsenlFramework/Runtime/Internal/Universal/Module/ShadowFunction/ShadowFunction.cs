using System;

namespace Hsenl {
    /// <summary>
    /// <para>影子系统分为两个部分, "源"和"影子", 影子函数相当于是源函数的外部实现.
    /// 二者不能在同一程序集内, 否则不起作用.
    /// 不支持out in, 不支持泛型函数, 支持async, 支持泛型类(仅支持一个参数)</para>
    ///
    /// <para>用途: 主要用于热重载, 让函数与数据分离, 也可以一定程度上平替Event系统</para>
    /// 
    /// <para>使用方法: 分两个程序集, 同一个程序集内无法使用.
    /// 在一个程序集里创建类Class1, 写方法Function1(){}, 分别给Class1, 和Function1添加[ShadowFunctionAttribute]特性, 且不做任何赋值, 如此该类和函数就被标记为了"源类和源函数".
    /// 再在另一个程序集里创建类Class1Shadow(名字不限), 写方法Function1(方法名必须和源函数完全相同), 给Class1Shadow添加[ShadowFunctionAttribute(typeof(源类))]特性.
    /// 如此准备工作就做好了, 在源类里, 会自动生成一个函数 -> {源函数名}Shadow的函数, 这个函数与我们的源函数参数完全一致, 我们只需要根据我们自己的需要去调用该函数, 影子函数就会被调用了.</para>
    ///
    /// <para>影子函数的名字和参数必须和源函数相同, 但参数有点例外, 就是如果你需要在影子里获取源类的实例的话, 可以把源类作为参数, 写在第一位, 代码生成时, 会自动忽略第一位参数, 然后再比较和源函数
    /// 的参数是否相同.</para>
    /// </summary>
    public static class ShadowFunction {
        public static void Register<T>(int hashcode, T del) where T : Delegate => ShadowFunctionManager.Instance.Register(hashcode, del);
        public static void Unregister(int hashcode) => ShadowFunctionManager.Instance.Unregister(hashcode);
        public static bool GetFunction(int hashcode, out Delegate dl) => ShadowFunctionManager.Instance.GetFunction(hashcode, out dl);
    }
}