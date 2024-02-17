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
    public class TaskLocker : IDisposable {
        private int _type;
        private long _key;
        private int _level;

        internal TaskLockManager taskLockManager;

        public static TaskLocker Create(int type, long k, int count) {
            var taskLocker = ObjectPool.Rent<TaskLocker>();
            taskLocker._type = type;
            taskLocker._key = k;
            taskLocker._level = count;
            return taskLocker;
        }

        public void Dispose() {
            this.taskLockManager.RunNext(this._type, this._key, this._level + 1);

            this._type = TaskLockType.None;
            this._key = 0;
            this._level = 0;

            ObjectPool.Return(this);
        }
    }
}