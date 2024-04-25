using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8603 // Possible null reference return.

namespace Hsenl {
    // HTask只是一个空壳子, 具体实现是由其内部的body来实现
    // 这么做的意义在于不让用户直接接触到body, 以防止用户保存body的引用, 从而造成未知的风险
    [AsyncMethodBuilder(typeof(AsyncHTaskMethodBuilder))]
    [StructLayout(LayoutKind.Auto)]
    public partial struct HTask : IHTask {
        private IHTaskBody? body;

        private HTask(IHTaskBody body) {
            this.body = body;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.body?.IsCompleted ?? true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() {
            // 无论如何, 使用过的task, 其body都会被清除, 确保body不会出现被重新使用而导致意料之外的错误, 例如body已经被回收, 但仍尝试使用它.
            // 使用过的task就像放过的烟花, 就是个空壳子, 本质上和HTask.Completed是一样的.
            var t = this.body;
            this.body = null;
            t?.SetResult();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            var t = this.body;
            this.body = null;
            t?.SetException(e);
        }

        // 一个类型想要能被await, 必须有GetAwaiter(), 并返回一个ICriticalNotifyCompletion
        // 一个类型想要能被async, 必须指定一个AsyncMethodBuilder
        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask _task;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask task) {
                this._task = task;
            }

            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task.body?.IsCompleted ?? true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() {
                this._task.body?.GetResult();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task.body?.OnCompleted(continuation);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task.body?.UnsafeOnCompleted(continuation);
            }
        }
    }


    [AsyncMethodBuilder(typeof(AsyncHTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public partial struct HTask<T> : IHTask {
        private IHTaskBody<T>? body;

        private HTask(IHTaskBody<T> body) {
            this.body = body;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.body?.IsCompleted ?? true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(T value) {
            var t = this.body;
            this.body = null;
            t?.SetResult(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            var t = this.body;
            this.body = null;
            t?.SetException(e);
        }

        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask<T> _task;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask<T> task) {
                this._task = task;
            }

            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task.body?.IsCompleted ?? true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetResult() {
                if (this._task.body == null)
                    return default;

                return this._task.body.GetResult();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task.body?.OnCompleted(continuation);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task.body?.UnsafeOnCompleted(continuation);
            }
        }
    }
}