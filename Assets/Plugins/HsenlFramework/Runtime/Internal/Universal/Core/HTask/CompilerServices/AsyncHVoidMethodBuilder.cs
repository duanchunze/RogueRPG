using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;

namespace Hsenl {
    public struct AsyncHVoidMethodBuilder {
        private IHTaskBody _body;

        // 1. Static Create method.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static AsyncHVoidMethodBuilder Create() => default;

        // 2. TaskLike Task property(void)
        public HVoid Task => default;

        // 3. SetException
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            if (this._body != null)
                HTaskPool.Return(this._body);

            if (e is HTaskAbortException)
                return;

            ExceptionDispatchInfo.Capture(e).Throw();
        }

        // 4. SetResult
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() {
            if (this._body != null)
                HTaskPool.Return(this._body);
        }

        // 5. AwaitOnCompleted
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : INotifyCompletion where TStateMachine : IAsyncStateMachine {
            this._body = HTaskPool.Rent<NormalHTaskBody>();
            this._body.StateMachine = stateMachine;
            awaiter.OnCompleted(this._body.MoveNext);
        }

        // 6. AwaitUnsafeOnCompleted
        [SecuritySafeCritical]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
            where TAwaiter : ICriticalNotifyCompletion where TStateMachine : IAsyncStateMachine {
            this._body = HTaskPool.Rent<NormalHTaskBody>();
            this._body.StateMachine = stateMachine;
            awaiter.UnsafeOnCompleted(this._body.MoveNext);
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