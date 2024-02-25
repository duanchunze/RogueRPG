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
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [Serializable]
    internal class TaskLockQueue {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private int _type;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private long _key;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private TaskLocker _currentTaskLock;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly Queue<WaitTaskLock> _queue = new();

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        public int Count => this._queue.Count;

        public static TaskLockQueue Create(int type, long key) {
            var taskLockQueue = ObjectPool.Rent<TaskLockQueue>();
            taskLockQueue._type = type;
            taskLockQueue._key = key;
            return taskLockQueue;
        }

        public async HTask<TaskLocker> Wait(int time, TaskLockManager taskLockManager) {
            if (this._currentTaskLock == null) {
                this._currentTaskLock = TaskLocker.Create(this._type, this._key, 1);
                this._currentTaskLock.taskLockManager = taskLockManager;
                return this._currentTaskLock;
            }

            var waitTaskLock = WaitTaskLock.Create();
            this._queue.Enqueue(waitTaskLock);

            async void CheckTimeOut(WaitTaskLock waitLock) {
                await Timer.WaitTime(time);
                if (!waitLock.IsDisposed()) {
                    waitLock.SetException(new Exception("task lock is timeout!"));
                }
            }

            if (time > 0) {
                CheckTimeOut(waitTaskLock);
            }

            this._currentTaskLock = await waitTaskLock.Wait();
            this._currentTaskLock.taskLockManager = taskLockManager;
            return this._currentTaskLock;
        }

        public void Notify(int level) {
            // 有可能 WaitTaskLock 已经超时抛出异常，所以要找到一个未处理的 WaitTaskLock
            while (this._queue.Count > 0) {
                var waitTaskLock = this._queue.Dequeue();

                if (waitTaskLock.IsDisposed()) {
                    continue;
                }

                var taskLock = TaskLocker.Create(this._type, this._key, level);

                waitTaskLock.SetResult(taskLock);
                break;
            }
        }

        public void Recycle() {
            this._queue.Clear();
            this._key = 0;
            this._type = 0;
            this._currentTaskLock = null;
            ObjectPool.Return(this);
        }
    }
}