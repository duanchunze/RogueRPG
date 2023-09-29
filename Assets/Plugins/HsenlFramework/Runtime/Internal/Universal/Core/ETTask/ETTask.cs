using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace Hsenl {
    [AsyncMethodBuilder(typeof(ETAsyncTaskMethodBuilder))]
    public class ETTask : ICriticalNotifyCompletion, IETTask {
        public static Action<Exception> ExceptionHandler;

        public static ETTaskCompleted CompletedTask => new();

        private static readonly Queue<ETTask> queue = new();

        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static ETTask Create(bool fromPool = false) {
            if (!fromPool) {
                return new ETTask();
            }

            if (queue.Count == 0) {
                return new ETTask() { fromPool = true };
            }

            return queue.Dequeue();
        }

        private void Recycle() {
            if (!this.fromPool) {
                return;
            }

            this.state = AwaiterStatus.Pending;
            this.callback = null;
            // 太多了
            if (queue.Count > 1000) {
                return;
            }

            queue.Enqueue(this);
        }

        private bool fromPool;
        private AwaiterStatus state;
        private object callback; // Action or ExceptionDispatchInfo

        private ETTask() { }

        [DebuggerHidden]
        private async ETVoid InnerCoroutine() {
            await this;
        }

        [DebuggerHidden]
        public void Coroutine() {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public ETTask GetAwaiter() {
            return this;
        }

        // await一开始就会进行一次完成判断，以确定是否需要挂起，如果否，则不需要执行回调代码打包的操作，提升代码效率，同样的，对于不需要挂起的await，代码执行也不会等到下一帧，而是直接执行下去
        public bool IsCompleted {
            [DebuggerHidden]
            get => this.state != AwaiterStatus.Pending;
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action) {
            if (this.state != AwaiterStatus.Pending) {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action) {
            this.UnsafeOnCompleted(action);
        }

        // 该方法的调用被包含在action里，当执行 c?.Invoke();时，会被调用
        [DebuggerHidden]
        public void GetResult() {
            switch (this.state) {
                case AwaiterStatus.Succeeded:
                    this.Recycle();
                    break;
                case AwaiterStatus.Faulted:
                    var c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    break;
                default:
                    throw new NotSupportedException("ETTask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }

        // 该方法我们可以主动调用，也可以系统自动调用，例如我们await一个函数，当函数执行完毕后（例如 return await tcs;） 当return时，系统内部会自动调用该方法
        [DebuggerHidden]
        public void SetResult() {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;

            var c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        // 后来自己添加的取消方法，并非dotnet自带的协议方法，所以，不会被自动调用
        // 特别警告, 取消代表该异步永远不会接着await继续执行了, 从而导致了, 如果是使用 WaitAll 的时候, 会永远等不到执行完毕
        [DebuggerHidden]
        public void Cancel() {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;
            // 这里直接取消，而不执行该callback，该callback被gc后，上一层的callback也会因为失去唯一引用而被gc，不会内存泄露
            this.callback = null;
            // 因为callback没有被执行，所以GetResult也不会被调用，需要我们自行调用
            this.GetResult();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [DebuggerHidden]
        public void SetException(Exception e) {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Faulted;

            var c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }

    [AsyncMethodBuilder(typeof(ETAsyncTaskMethodBuilder<>))]
    public class ETTask<T> : ICriticalNotifyCompletion, IETTask {
        private static readonly Queue<ETTask<T>> queue = new Queue<ETTask<T>>();

        /// <summary>
        /// 请不要随便使用ETTask的对象池，除非你完全搞懂了ETTask!!!
        /// 假如开启了池,await之后不能再操作ETTask，否则可能操作到再次从池中分配出来的ETTask，产生灾难性的后果
        /// SetResult的时候请现将tcs置空，避免多次对同一个ETTask SetResult
        /// </summary>
        public static ETTask<T> Create(bool fromPool = false) {
            if (!fromPool) {
                return new ETTask<T>();
            }

            if (queue.Count == 0) {
                return new ETTask<T>() { fromPool = true };
            }

            return queue.Dequeue();
        }

        private void Recycle() {
            if (!this.fromPool) {
                return;
            }

            this.callback = null;
            this.value = default;
            this.state = AwaiterStatus.Pending;
            // 太多了
            if (queue.Count > 1000) {
                return;
            }

            queue.Enqueue(this);
        }

        private bool fromPool;
        private AwaiterStatus state;
        private T value;
        private object callback; // Action or ExceptionDispatchInfo

        private ETTask() { }

        [DebuggerHidden]
        private async ETVoid InnerCoroutine() {
            await this;
        }

        [DebuggerHidden]
        public void Coroutine() {
            InnerCoroutine().Coroutine();
        }

        [DebuggerHidden]
        public ETTask<T> GetAwaiter() {
            return this;
        }

        [DebuggerHidden]
        public T GetResult() {
            switch (this.state) {
                case AwaiterStatus.Succeeded:
                    var v = this.value;
                    this.Recycle();
                    return v;
                case AwaiterStatus.Faulted:
                    var c = this.callback as ExceptionDispatchInfo;
                    this.callback = null;
                    this.Recycle();
                    c?.Throw();
                    return default;
                default:
                    throw new NotSupportedException("ETask does not allow call GetResult directly when task not completed. Please use 'await'.");
            }
        }


        public bool IsCompleted {
            [DebuggerHidden]
            get { return state != AwaiterStatus.Pending; }
        }

        [DebuggerHidden]
        public void UnsafeOnCompleted(Action action) {
            if (this.state != AwaiterStatus.Pending) {
                action?.Invoke();
                return;
            }

            this.callback = action;
        }

        [DebuggerHidden]
        public void OnCompleted(Action action) {
            this.UnsafeOnCompleted(action);
        }

        [DebuggerHidden]
        public void SetResult(T result) {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;

            this.value = result;

            var c = this.callback as Action;
            this.callback = null;
            c?.Invoke();
        }

        [DebuggerHidden]
        public void Cancel() {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Succeeded;
            this.value = default;
            // 这里直接取消，而不执行该callback，该callback被gc后，上一层的callback也会因为失去唯一引用而被gc，不会内存泄露
            this.callback = null;
            this.GetResult();
        }

        [DebuggerHidden]
        public void SetException(Exception e) {
            if (this.state != AwaiterStatus.Pending) {
                throw new InvalidOperationException("TaskT_TransitionToFinal_AlreadyCompleted");
            }

            this.state = AwaiterStatus.Faulted;

            var c = this.callback as Action;
            this.callback = ExceptionDispatchInfo.Capture(e);
            c?.Invoke();
        }
    }
}