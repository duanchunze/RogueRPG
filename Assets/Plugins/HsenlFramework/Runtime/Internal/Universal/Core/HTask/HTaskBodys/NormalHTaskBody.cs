using System;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8603 // Possible null reference return.

namespace Hsenl {
    internal class NormalHTaskBody : IHTaskBody {
        private uint _version;
        private HTaskStatus _status;
        private Action? _continuation;
        private ExceptionDispatchInfo? _exception;

        public uint Version => this._version;
        HTaskStatus IHTaskBody.Status => this._status;
        bool IHTaskBody.IsCompleted => this._status != HTaskStatus.Pending;
        public bool IsCompleted => ((IHTaskBody)this).IsCompleted;
        public Action MoveNext { get; }
        public IAsyncStateMachine StateMachine { get; set; }

        public NormalHTaskBody() {
            this.MoveNext = this.Run;
        }

        void IHTaskBody.GetResult() {
            if (this._exception != null) {
                var e = this._exception;
                this._exception = null;
                HTaskPool.Return(this);
                e.Throw();
            }
            else {
                var s = this._status;
                HTaskPool.Return(this);
                if (s == HTaskStatus.Aborted)
                    throw HTaskAbortException.GetException();
            }
        }

        void IHTaskBody.SetResult() {
            this.IncrementVersion();
            this._status = HTaskStatus.Succeeded;
            var c = this._continuation;
            if (c != null) {
                this._continuation = null;
                c.Invoke();
            }
            else {
                // 因为GetResult已经不会被调用了, 所以这里就提前把自己回收
                HTaskPool.Return(this);
            }
        }

        void IHTaskBody.SetException(Exception e) {
            this.IncrementVersion();
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            if (c != null) {
                this._continuation = null;
                this._exception = ExceptionDispatchInfo.Capture(e);
                c.Invoke();
            }
            else {
                // 例如下面这种情况
                // async HTask Fun(){}
                // void Test()
                // {
                //    Fun().Tial();
                // }
                // 我们拿到Fun返回的task, 但却没有继续await他, 这就导致了这里的this._continuation是空的, 而由此也导致不会触发上面的GetResult(), 也就导致了
                // exception不会传下去, 也就导致了"异常被吞了"的情况. 面对这种情况, 我们直接在这里就把异常抛出去.
                var s = this._status;
                HTaskPool.Return(this);
                if (s != HTaskStatus.Aborted)
                    ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        void IHTaskBody.Abort() {
            this.IncrementVersion();
            this._status = HTaskStatus.Aborted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody.OnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        void IHTaskBody.UnsafeOnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementVersion() {
            this._version++;
            if (this._version > HTask.MaxVersion)
                this._version = 0;
        }
        
        private void Run() {
            this.StateMachine.MoveNext();
        }

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
            this.StateMachine = null;
        }
    }

    internal class NormalHTaskBody<T> : IHTaskBody<T> {
        private uint _version;
        private HTaskStatus _status;
        private Action? _continuation;
        private ExceptionDispatchInfo? _exception;

        private T _value;

        public uint Version => this._version;
        HTaskStatus IHTaskBody<T>.Status => this._status;
        bool IHTaskBody<T>.IsCompleted => this._status != HTaskStatus.Pending;
        public bool IsCompleted => ((IHTaskBody<T>)this).IsCompleted;
        public Action MoveNext { get; }
        public IAsyncStateMachine StateMachine { get; set; }

        public NormalHTaskBody() {
            this.MoveNext = this.Run;
        }

        T IHTaskBody<T>.GetResult() {
            var v = this._value;
            this._value = default;
            if (this._exception != null) {
                var e = this._exception;
                this._exception = null;
                HTaskPool<T>.Return(this);
                e.Throw();
            }
            else {
                var s = this._status;
                HTaskPool<T>.Return(this);
                if (s == HTaskStatus.Aborted)
                    throw HTaskAbortException.GetException();
            }

            return v;
        }

        void IHTaskBody<T>.SetResult(T value) {
            this.IncrementVersion();
            this._status = HTaskStatus.Succeeded;
            this._value = value;
            var c = this._continuation;
            if (c != null) {
                this._continuation = null;
                c.Invoke();
            }
            else {
                HTaskPool<T>.Return(this);
            }
        }

        void IHTaskBody<T>.SetException(Exception e) {
            this.IncrementVersion();
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            if (c != null) {
                this._continuation = null;
                this._exception = ExceptionDispatchInfo.Capture(e);
                c.Invoke();
            }
            else {
                var s = this._status;
                HTaskPool<T>.Return(this);
                if (s != HTaskStatus.Aborted)
                    ExceptionDispatchInfo.Capture(e).Throw();
            }
        }

        void IHTaskBody<T>.Abort() {
            this.IncrementVersion();
            this._status = HTaskStatus.Aborted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody<T>.OnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        void IHTaskBody<T>.UnsafeOnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IncrementVersion() {
            this._version++;
            if (this._version > HTask.MaxVersion)
                this._version = 0;
        }

        private void Run() {
            this.StateMachine.MoveNext();
        }

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
            this.StateMachine = null;
            this._value = default;
        }
    }
}