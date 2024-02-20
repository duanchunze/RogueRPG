using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security;

namespace Hsenl {
    public struct AsyncHVoidMethodBuilder {
        // 1. Static Create method.
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncHVoidMethodBuilder Create() => default;

        // 2. TaskLike Task property(void)
        public HVoid Task => default;

        // 3. SetException
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            if (e is HTaskAbortException)
                return;

            HTask.ExceptionHandler?.Invoke(e);
        }

        // 4. SetResult
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() { }

        // 5. AwaitOnCompleted
        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [DebuggerHidden]
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine {
            awaiter.OnCompleted(stateMachine.MoveNext);
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