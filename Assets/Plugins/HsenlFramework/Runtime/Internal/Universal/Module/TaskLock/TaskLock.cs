namespace Hsenl {
    public static class TaskLock {
        public static async ETTask<TaskLocker> Wait(int taskLockType, long key, int time = 60000) =>
            await TaskLockManager.Instance.Wait(taskLockType, key, time);
    }
}