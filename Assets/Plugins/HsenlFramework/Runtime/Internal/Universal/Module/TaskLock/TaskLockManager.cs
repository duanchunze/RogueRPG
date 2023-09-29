using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class TaskLockManager : Singleton<TaskLockManager> {
        [ShowInInspector, ReadOnly]
        private readonly List<TaskLockQueueType> _list = new();

        private readonly Queue<(int, long, int)> _nextFrameRun = new();

        public TaskLockManager() {
            for (var i = 0; i < TaskLockType.Max; ++i) {
                var queueType = new TaskLockQueueType(i);
                this._list.Add(queueType);
            }
        }

        protected override void Dispose() {
            base.Dispose();

            this._list.Clear();
            this._nextFrameRun.Clear();
        }

        public void Update() {
            // 循环过程中会有对象继续加入队列
            while (this._nextFrameRun.Count > 0) {
                var (coroutineLockType, key, count) = this._nextFrameRun.Dequeue();
                this.Notify(coroutineLockType, key, count);
            }
        }

        internal void RunNext(int taskLockType, long key, int level) {
            // 一个协程队列一帧处理超过100个,说明比较多了,打个warning,检查一下是否够正常
            if (level == 100) {
                Log.Warning($"too much task lock level: {taskLockType} {key}");
            }

            this._nextFrameRun.Enqueue((taskLockType, key, level));
        }

        public async ETTask<TaskLocker> Wait(int taskLockType, long key, int time = 60000) {
            var queueType = this._list[taskLockType];
            return await queueType.Wait(key, time, this);
        }

        private void Notify(int taskLockType, long key, int level) {
            var queueType = this._list[taskLockType];
            queueType.Notify(key, level);
        }
    }
}