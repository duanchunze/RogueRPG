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
        public const uint MaxVersion = uint.MaxValue - 2;

        private IHTaskBody? _body;
        private readonly uint _version;
        
        internal IHTaskBody Body => this._body;

        private HTask(IHTaskBody body) {
            this._body = body;
            this._version = body.Version;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._body?.IsCompleted ?? true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult() {
            // 无论如何, 使用过的task, 其body都会被清除, 确保body不会出现被重新使用而导致意料之外的错误, 例如body已经被回收, 但仍尝试使用它.
            // 使用过的task就像放过的烟花, 就是个空壳子, 本质上和HTask.Completed是一样的.
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    // 说明body已经被SetResult()了, 但仍然想要再次SetResult(). 正常情况下, 被SetResult()后的body会被自动移除, 但如果是用户自行保存了HTask, 且使用的是HTask的拷贝副本
                    // 做的SetResult, 就可能会出现这种情况. 检查下, 是否有使用list等保存HTask, 却在使用后, 忘记及时更新的情况出现.
                    throw new Exception("HTask's body is expired!");
                }

                t.SetResult();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    throw new Exception("HTask's body is expired!");
                }

                t.SetException(e);
            }
        }

        // 一个类型想要能被await, 必须有GetAwaiter()函数, 拓展的也行, 并返回一个ICriticalNotifyCompletion
        // 一个类型想要能被async, 必须指定一个AsyncMethodBuilder
        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask _task;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask task) {
                this._task = task;
            }

            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task._body?.IsCompleted ?? true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void GetResult() {
                this._task._body?.GetResult();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task._body!.OnCompleted(continuation);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task._body!.UnsafeOnCompleted(continuation);
            }
        }
    }


    [AsyncMethodBuilder(typeof(AsyncHTaskMethodBuilder<>))]
    [StructLayout(LayoutKind.Auto)]
    public partial struct HTask<T> : IHTask {
        private IHTaskBody<T>? _body;
        private readonly uint _version;
        private T _value;

        internal IHTaskBody<T> Body => this._body;

        private HTask(IHTaskBody<T> body) {
            this._body = body;
            this._version = body.Version;
            this._value = default;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter GetAwaiter() {
            return new Awaiter(this);
        }

        public bool IsCompleted {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this._body?.IsCompleted ?? true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult(T value) {
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    // 说明body已经被SetResult()了, 但仍然想要再次SetResult(). 正常情况下, 被SetResult()后的body会被自动移除, 但如果是用户自行保存了HTask, 且使用的是HTask的拷贝副本
                    // 做的SetResult, 就可能会出现这种情况. 检查下, 是否有使用list等保存HTask, 却在使用后, 忘记及时更新的情况出现.
                    throw new Exception("HTask's body is expired!");
                }

                t.SetResult(value);
            }
            else {
                this._value = value;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e) {
            var t = this._body;
            if (t != null) {
                this._body = null;
                if (this._version != t.Version) {
                    throw new Exception("HTask's body is expired!");
                }

                t.SetException(e);
            }
        }

        public readonly struct Awaiter : ICriticalNotifyCompletion {
            private readonly HTask<T> _task;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Awaiter(in HTask<T> task) {
                this._task = task;
            }

            public bool IsCompleted {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this._task._body?.IsCompleted ?? true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T GetResult() {
                // 假如task没有被挂起的话, 会先调用SetResult(value), 然后在调用GetAwaiter(), value自然会被传过来, 
                // 但如果task被挂起的话, 会先调用GetAwaiter(), SetResult(value)被在之后被调用, 此时value自然传不过来, 因为HTask是结构体
                // 所以针对value, 设计了两种方式, 如果挂起了, 自然就会有body, 那么就把value存在body里, 因为body是引用类型, 如果没挂起, 就不会有body, 那就把值存在HTask里.
                // 详情可以看上面SetResult()方法中, 对保存value的写法.
                if (this._task._body == null)
                    return this._task._value;

                return this._task._body.GetResult();
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void OnCompleted(Action continuation) {
                this._task._body!.OnCompleted(continuation);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void UnsafeOnCompleted(Action continuation) {
                this._task._body!.UnsafeOnCompleted(continuation);
            }
        }
    }
}