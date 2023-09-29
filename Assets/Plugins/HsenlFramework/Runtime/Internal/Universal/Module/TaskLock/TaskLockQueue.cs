using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    internal class TaskLockQueue {
        [ShowInInspector, ReadOnly]
        private int _type;

        [ShowInInspector, ReadOnly]
        private long _key;

        [ShowInInspector, ReadOnly]
        private TaskLocker _currentTaskLock;

        [ShowInInspector, ReadOnly]
        private readonly Queue<WaitTaskLock> _queue = new();

        [ShowInInspector]
        public int Count => this._queue.Count;

        public static TaskLockQueue Create(int type, long key) {
            var taskLockQueue = ObjectPool.Fetch<TaskLockQueue>();
            taskLockQueue._type = type;
            taskLockQueue._key = key;
            return taskLockQueue;
        }

        public async ETTask<TaskLocker> Wait(int time, TaskLockManager taskLockManager) {
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
            ObjectPool.Recycle(this);
        }
    }
}