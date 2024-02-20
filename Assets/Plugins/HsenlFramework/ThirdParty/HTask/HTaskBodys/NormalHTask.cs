using System;
using System.Runtime.ExceptionServices;
#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8603 // Possible null reference return.

namespace Hsenl {
    internal class NormalHTask : IHTask {
        protected HTaskStatus _status;
        protected Action? _continuation;
        protected ExceptionDispatchInfo? _exception;

        HTaskStatus IHTask.Status => this._status;

        bool IHTask.IsCompleted => this._status != HTaskStatus.Pending;

        void IHTask.GetResult() {
            if (this._exception != null) {
                var e = this._exception;
                this._exception = null;
                HTaskPool.Return(this);
                e.Throw();
            }
            else {
                var s = this._status;
                HTaskPool.Return(this);
                switch (s) {
                    case HTaskStatus.Aborted:
                        throw new HTaskAbortException();
                }
            }
        }

        void IHTask.SetResult() {
            this._status = HTaskStatus.Succeeded;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask.SetException(Exception e) {
            this._exception = ExceptionDispatchInfo.Capture(e);
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask.Abort() {
            this._status = HTaskStatus.Aborted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask.OnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        void IHTask.UnsafeOnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
        }
    }

    internal class NormalHTask<T> : IHTask<T> {
        protected HTaskStatus _status;
        protected Action? _continuation;
        protected ExceptionDispatchInfo? _exception;

        protected T _value;

        HTaskStatus IHTask<T>.Status => this._status;

        bool IHTask<T>.IsCompleted => this._status != HTaskStatus.Pending;

        T IHTask<T>.GetResult() {
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
                switch (s) {
                    case HTaskStatus.Aborted:
                        throw new HTaskAbortException();
                }
            }

            return v;
        }

        void IHTask<T>.SetResult(T value) {
            this._status = HTaskStatus.Succeeded;
            this._value = value;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask<T>.SetException(Exception e) {
            this._exception = ExceptionDispatchInfo.Capture(e);
            this._status = e is HTaskAbortException ? HTaskStatus.Aborted : HTaskStatus.Faulted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask<T>.Abort() {
            this._status = HTaskStatus.Aborted;
            var c = this._continuation;
            this._continuation = null;
            c?.Invoke();
        }

        void IHTask<T>.OnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        void IHTask<T>.UnsafeOnCompleted(Action continuation) {
            this._continuation = continuation;
        }

        public void Dispose() {
            this._status = HTaskStatus.Pending;
            this._continuation = null;
            this._exception = null;
        }
    }
}