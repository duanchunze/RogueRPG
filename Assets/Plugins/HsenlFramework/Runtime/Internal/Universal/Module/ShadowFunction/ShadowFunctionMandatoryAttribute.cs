﻿// using System;
//
// namespace Hsenl {
//     /// <summary>
//     /// 强制标记为影子函数, 只在对影子函数时, 才有作用.
//     /// 自动脚本生成无法获取第三方库中的私有成员, 例如影子类中, 如果某个影子函数对应的源函数是一个私有函数的话, 那自动脚本将无法识别, 这时, 我们可以给它添加这个标记, 那么它会被强制生成.
//     /// 除此外, 在面对泛型类的时候, 也需要用到该特性.
//     ///
//     /// 其原理就是简单的把当前影子函数, 加个key, 直接保存到影子函数管理器中, 而不做连接的有效性检查, 所以他总能成功.
//     ///
//     /// 如无必要, 不要使用整个特性, 因为他会让你不知道某个影子函数有没有正确连接上源函数, 即便没连上, 他也不会报错
//     /// </summary>
//     [AttributeUsage(AttributeTargets.Method, Inherited = false)]
//     public class ShadowFunctionMandatoryAttribute : BaseAttribute {
//         public int priority;
//
//         public ShadowFunctionMandatoryAttribute(int priority = 0) {
//             this.priority = priority;
//         }
//     }
// }

// 由于symbol无法获取其他程序集的私有成员, 所以之前用该特性来强制标记影子函数, 用以支持源函数是private的情况, 但问题是, 如此便无法了解是否连接成功.
// 现在用了另一种方案, 在源程序集自动生成一个专门用于查询的表单, 如此影子程序集就能获取所有需要的信息了, 自然也就不需要该特性了