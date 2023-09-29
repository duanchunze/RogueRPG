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