using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public partial struct HTask {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsNull() => this.body == null;

        // 小尾巴, 让编译器不警告的, 对实际运行没影响
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        // 终止整条异步线. (只对异步有效, 一般来说, task肯定是异步, 但不排除你创建一个task, 然后赶在await之前SetResult)
        // 实质上是通过抛出一个自定义的异常来实现终止的, 所以难免会多消耗资源, 所以如无必要, 不要频繁使用该方法.
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