/*MIT License

Copyright (c) 2018 tanghai

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using System;

namespace Hsenl {
    internal class WaitTaskLock {
        public static WaitTaskLock Create() {
            var waitTaskLock = new WaitTaskLock {
                _tcs = HTask<TaskLocker>.Create()
            };
            return waitTaskLock;
        }

        private HTask<TaskLocker>? _tcs;

        public void SetResult(TaskLocker taskLock) {
            if (this._tcs == null) {
                throw new NullReferenceException("SetResult tcs is null");
            }

            var t = this._tcs;
            this._tcs = null;
            t.Value.SetResult(taskLock);
        }

        public void SetException(Exception exception) {
            if (this._tcs == null) {
                throw new NullReferenceException("SetException tcs is null");
            }

            var t = this._tcs;
            this._tcs = null;
            t.Value.SetException(exception);
        }

        public bool IsDisposed() {
            return this._tcs == null;
        }

        public async HTask<TaskLocker> Wait() {
            if (this._tcs == null)
                return null;

            return await this._tcs.Value;
        }
    }
}