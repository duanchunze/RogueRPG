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