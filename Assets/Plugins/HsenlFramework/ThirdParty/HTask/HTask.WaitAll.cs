using System.Collections.Generic;

namespace Hsenl {
    public partial struct HTask {
        public static async HTask WaitAll(IList<HTask> tasks) {
            if (tasks.Count == 0)
                return;

            var wait = Create();
            var count = tasks.Count;
            for (int i = 0, len = tasks.Count; i < len; i++) {
                Wait(tasks[i]).Tail();
            }

            async HVoid Wait(HTask task) {
                await task;
                count--;
                if (count == 0) {
                    wait.SetResult();
                }
            }

            if (wait.IsCompleted)
                return;

            await wait;
        }
    }

    public partial struct HTask<T> {
        public static async HTask WaitAll(IList<HTask<T>> tasks) {
            if (tasks.Count == 0)
                return;

            var wait = HTask.Create();
            var count = tasks.Count;
            for (int i = 0, len = tasks.Count; i < len; i++) {
                Wait(tasks[i]).Tail();
            }

            async HVoid Wait(HTask<T> task) {
                await task;
                count--;
                if (count == 0) {
                    wait.SetResult();
                }
            }

            if (wait.IsCompleted)
                return;

            await wait;
        }
    }
}