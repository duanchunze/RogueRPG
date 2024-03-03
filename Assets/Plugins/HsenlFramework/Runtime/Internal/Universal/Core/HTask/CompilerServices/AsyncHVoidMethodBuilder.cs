using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;

namespace Hsenl {
    public struct AsyncHVoidMethodBuilder {
        // 1. Static Create method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncHVoidMethodBuilder Create() => default;

        // 2. TaskLike Task property(void)
        public HVoid Task => default;

        // 3. SetException
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            if (e is HTaskAbortException)
                return;

            ExceptionDispatchInfo.Capture(e).Throw();
        }

        // 4. SetResult
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() { }

        // 5. AwaitOnCompleted
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine {
            awaiter.OnCompleted(stateMachine.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine {
            awaiter.UnsafeOnCompleted(stateMachine.MoveNext);
        }

        // 7. Start
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine {
            stateMachine.MoveNext();
        }

        // 8. SetStateMachine
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetStateMachine(IAsyncStateMachine stateMachine) { }
    }
}