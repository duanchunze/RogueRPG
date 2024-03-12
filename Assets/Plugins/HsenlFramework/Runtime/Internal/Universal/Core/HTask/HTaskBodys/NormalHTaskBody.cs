using System;
using System.Runtime.ExceptionServices;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8603 // Possible null reference return.

namespace Hsenl {
    internal class NormalHTaskBody : IHTaskBody, IHTask {
        private HTaskStatus _status;
        private Action? _continuation;
        private ExceptionDispatchInfo? _exception;

        HTaskStatus IHTaskBody.Status => this._status;

        bool IHTaskBody.IsCompleted => this._status != HTaskStatus.Pending;

        public bool IsCompleted => ((IHTaskBody)this).IsCompleted;

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
                if (s == HTaskStatus.Aborted) throw HTaskAbortException.GetException();
            }
        }

        void IHTaskBody.SetResult() {
            this._status = HTaskStatus.Succeeded;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody.SetException(Exception e) {
            this._exception = ExceptionDispatchInfo.Capture(e);
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody.Abort() {
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

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
        }
    }

    internal class NormalHTaskBody<T> : IHTaskBody<T>, IHTask {
        private HTaskStatus _status;
        private Action? _continuation;
        private ExceptionDispatchInfo? _exception;

        private T _value;

        HTaskStatus IHTaskBody<T>.Status => this._status;

        bool IHTaskBody<T>.IsCompleted => this._status != HTaskStatus.Pending;
        
        public bool IsCompleted => ((IHTaskBody<T>)this).IsCompleted;

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
                if (s == HTaskStatus.Aborted) throw HTaskAbortException.GetException();
            }

            return v;
        }

        void IHTaskBody<T>.SetResult(T value) {
            this._status = HTaskStatus.Succeeded;
            this._value = value;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody<T>.SetException(Exception e) {
            this._exception = ExceptionDispatchInfo.Capture(e);
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTaskBody<T>.Abort() {
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

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
        }
    }
}