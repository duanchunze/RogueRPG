using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public partial struct HTask {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => this.body == null;

        // 小尾巴, 让编译器不警告的, 对实际运行没影响
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        // 终止整条异步线.
        // 只有当task被实际挂起了才有效, 毕竟task都同步执行了, 你哪还有机会去abort, 一般来说, 我们用task肯定会异步,
        // 但不排除你创建一个task, 然后赶在await之前就提前SetResult, 虽然这么做完全没什么意义.
        // 原理是通过在内部抛出一个自定义的异常来实现终止的, 因为await\async并没有留出真正用来实现abort的途径.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            this.body?.Abort();
        }
    }

    public partial struct HTask<T> {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => this.body == null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            this.body?.Abort();
        }
    }
}