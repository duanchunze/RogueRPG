using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace Hsenl {
    // 当函数是 async HTask 时, 由该Builder来创建并操作task
    [StructLayout(LayoutKind.Auto)]
    public struct AsyncHTaskMethodBuilder {
        private HTask _task;

        // 1. Static Create method.
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncHTaskMethodBuilder Create() => default;

        // 2. TaskLike Task property(void)
        public HTask Task {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._task; // 如果AwaitOnCompleted没被调用, 那说明没挂起, 这里的task也只是个空壳子, 上级的 await 相当于 await 了一个 HTask.Completed;
        }

        // 3. SetException
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            this._task.SetException(e);
        }

        // 4. SetResult
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() {
            this._task.SetResult();
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine {
            // 该方法被调用时, 说明状态机被挂起了, 所以要创建一个task, 用于保存continuation, 不然回调链会断掉
            this._task = HTask.Create();
            // stateMachine因为awaiter被挂起了, 所以当awaiter完成时, 回调通知stateMachine继续
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine {
            this.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        // 7. Start
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    }


    [StructLayout(LayoutKind.Auto)]
    public struct AsyncHTaskMethodBuilder<T> {
        private HTask<T> _task;

        // 1. Static Create method.
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncHTaskMethodBuilder<T> Create() => default;

        // 2. TaskLike Task property(void)
        public HTask<T> Task {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._task;
        }

        // 3. SetException
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            this._task.SetException(e);
        }

        // 4. SetResult
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(T value) {
            this._task.SetResult(value);
        }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine {
            this._task = HTask<T>.Create();
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine {
            this.AwaitOnCompleted(ref awaiter, ref stateMachine);
        }

        // 7. Start
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    }
}