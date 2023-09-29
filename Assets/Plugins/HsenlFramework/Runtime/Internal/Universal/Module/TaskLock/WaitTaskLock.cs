using System;

namespace Hsenl {
    internal class WaitTaskLock {
        public static WaitTaskLock Create() {
            var waitTaskLock = new WaitTaskLock {
                _tcs = ETTask<TaskLocker>.Create(true)
            };
            return waitTaskLock;
        }

        private ETTask<TaskLocker> _tcs;

        public void SetResult(TaskLocker taskLock) {
            if (this._tcs == null) {
                throw new NullReferenceException("SetResult tcs is null");
            }

            var t = this._tcs;
            this._tcs = null;
            t.SetResult(taskLock);
        }

        public void SetException(Exception exception) {
            if (this._tcs == null) {
                throw new NullReferenceException("SetException tcs is null");
            }

            var t = this._tcs;
            this._tcs = null;
            t.SetException(exception);
        }

        public bool IsDisposed() {
            return this._tcs == null;
        }

        public async ETTask<TaskLocker> Wait() {
            return await this._tcs;
        }
    }
}