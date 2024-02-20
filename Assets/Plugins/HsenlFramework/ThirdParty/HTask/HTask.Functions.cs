using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public partial struct HTask {
        // 小尾巴, 让编译器不警告的, 对实际运行没影响
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        // 终止整条异步线. (只对异步有效, 一般来说, task肯定是异步, 但不排除你创建一个task, 然后赶在await之前SetResult)
        // 实质上是通过抛出一个自定义的异常来实现终止的, 所以难免会多消耗资源, 所以如无必要, 不要频繁使用该方法.
        // 还有另一种中断的方式, 使用创建cancelable的形式, 那个无性能损耗. 那个要手动拿到返回的bool, 自己去写逻辑, 例如, if (canceled) 则不执行...
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            this.body?.Abort();
        }
    }

    public partial struct HTask<T> {
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Tail() { }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Abort() {
            this.body?.Abort();
        }
    }
}