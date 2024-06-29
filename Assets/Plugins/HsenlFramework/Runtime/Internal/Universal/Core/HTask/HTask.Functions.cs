using System;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public partial struct HTask {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => this._body == null;

        // 小尾巴, 让编译器不警告的, 对实际运行没影响
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        // 终止整条异步线.
        // 只有当task被实际挂起了才有效, 毕竟task都同步执行了, 就没有机会去abort了, 一般来说, 我们用task肯定会异步,
        // 但不排除你创建一个task, 然后赶在await之前就提前SetResult, 虽然这么做完全没什么意义.
        // 原理是通过在内部抛出一个自定义的异常来实现终止的, 因为await\async并没有留出真正用来实现abort的途径, 正因使用抛异常的方式, 所以其效果和throw exception一样的, 只不过最终不会真的
        // 把异常给抛出来.
        // 同时使用abort会对于效率会有一定影响, 因为毕竟要throw异常, 如果是性能十分敏感的地方, 可以不使用abort, 或者使用其他方案, 比如task<bool>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    // 说明body已经被SetResult()了, 但仍然想要再次SetResult(). 正常情况下, 被SetResult()后的body会被自动移除, 但如果是用户自行保存了HTask, 且使用的是HTask的拷贝副本
                    // 做的SetResult, 就可能会出现这种情况. 检查下, 是否有使用list等保存HTask, 却在使用后, 忘记及时更新的情况出现.
                    throw new Exception("HTask's body is expired!");
                }

                t.Abort();
            }
        }
    }

    public partial struct HTask<T> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => this._body == null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    throw new Exception("HTask's body is expired!");
                }

                t.Abort();
            }
        }
    }
}