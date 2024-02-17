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
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    internal class TaskLockQueueType {
        [ShowInInspector, ReadOnly]
        private readonly int _type; // 协程锁类型

        [ShowInInspector, ReadOnly]
        private readonly Dictionary<long, TaskLockQueue> _taskLockQueues = new(); // key 和 协程锁列队

        public TaskLockQueueType(int type) {
            this._type = type;
        }

        private TaskLockQueue Get(long key) {
            this._taskLockQueues.TryGetValue(key, out var queue);
            return queue;
        }

        private TaskLockQueue New(long key) {
            var queue = TaskLockQueue.Create(this._type, key);
            this._taskLockQueues.Add(key, queue);
            return queue;
        }

        private void Remove(long key) {
            if (this._taskLockQueues.Remove(key, out var queue)) {
                queue.Recycle();
            }
        }

        public async ETTask<TaskLocker> Wait(long key, int time, TaskLockManager taskLockManager) {
            var queue = this.Get(key) ?? this.New(key);
            return await queue.Wait(time, taskLockManager);
        }

        public void Notify(long key, int level) {
            var queue = this.Get(key);
            if (queue == null) {
                return;
            }

            if (queue.Count == 0) {
                this.Remove(key);
            }

            queue.Notify(level);
        }
    }
}