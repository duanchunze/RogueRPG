using System;
using System.Diagnostics.CodeAnalysis;

namespace Hsenl {
    // 异常是个好东西，但是也会对效率造成影响。因为异常在代码中通常是不常见的，因为 JIT 在编译代码时，会将包含抛出异常的代码认定为冷块（即不会被怎么执行的代码块），
    // 这么一来会影响 inline 的决策：
    // void Foo()
    // {
    //     // ...
    //     throw new Exception();
    // }
    // 例如上面这个 Foo 方法，就很难被 inline 掉
    // 但是，我们可以将异常拿走放到单独的方法中抛出，这么一来，抛异常的行为就被我们转换成了普通的函数调用行为，于是就不会影响对 Foo 的 inline 优化，将冷块从 Foo 转移到了 Throw 中：
    // [DoesNotReturn] void Throw() => throw new Exception();
    //
    // void Foo()
    // {
    //     // ...
    //     Throw();
    // }
    // 摘取自 https://zhuanlan.zhihu.com/p/579403949
    public static class Assert {
        // 遇到需要inline函数的时候, 使用这种抛异常方式
        [DoesNotReturn]
        public static void NullReference(in string message) => throw new NullReferenceException(message);

        [DoesNotReturn]
        public static void NullReference(object o, in string message) {
            if (o == null) {
                throw new NullReferenceException(message);
            }
        }
    }
}