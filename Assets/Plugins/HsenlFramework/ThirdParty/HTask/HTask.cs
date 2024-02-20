using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8603 // Possible null reference return.

namespace Hsenl {
    // HTask只是一个空壳子, 具体实现是由其内部的body来实现
    [AsyncMethodBuilder(typeof(AsyncHTaskMethodBuilder))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct HTask {
        public static Action<Exception> ExceptionHandler;
        
        internal readonly IHTask? body;

        internal HTask(IHTask body) {
            this.body = body;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.body?.IsCompleted ?? true;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() {
            this.body?.SetResult();
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            this.body?.SetException(e);
        }

        // 一个类型想要能被await, 必须有GetAwaiter(), 并返回一个ICriticalNotifyCompletion
        // 一个类型想要能被async, 必须指定一个AsyncMethodBuilder
        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask _task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask task) {
                this._task = task;
            }

            [DebuggerHidden]
            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task.body?.IsCompleted ?? true;
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() {
                this._task.body?.GetResult();
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task.body?.OnCompleted(continuation);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task.body?.UnsafeOnCompleted(continuation);
            }
        }
    }


    [AsyncMethodBuilder(typeof(AsyncHTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public readonly partial struct HTask<T> {
        internal readonly IHTask<T>? body;

        internal HTask(IHTask<T> body) {
            this.body = body;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.body?.IsCompleted ?? true;
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(T value) {
            this.body?.SetResult(value);
        }

        [DebuggerHidden]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            this.body?.SetException(e);
        }

        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask<T> _task;

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask<T> task) {
                this._task = task;
            }

            [DebuggerHidden]
            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task.body?.IsCompleted ?? true;
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetResult() {
                if (this._task.body == null)
                    return default;

                return this._task.body.GetResult();
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task.body?.OnCompleted(continuation);
            }

            [DebuggerHidden]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task.body?.UnsafeOnCompleted(continuation);
            }
        }
    }
}