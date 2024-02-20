using System.Collections.Generic;

namespace Hsenl {
    public partial struct HTask {
        public static async HTask WaitAny(IList<HTask> tasks) {
            if (tasks.Count == 0)
                return;

            var wait = Create();
            for (int i = 0, len = tasks.Count; i < len; i++) {
                Wait(tasks[i]).Tail();
            }

            async HVoid Wait(HTask task) {
                await task;
                wait.SetResult();
            }

            if (wait.IsCompleted)
                return;

            await wait;
        }
    }
    
    public partial struct HTask<T> {
        public static async HTask WaitAny(IList<HTask<T>> tasks) {
            if (tasks.Count == 0)
                return;

            var wait = HTask.Create();
            for (int i = 0, len = tasks.Count; i < len; i++) {
                Wait(tasks[i]).Tail();
            }

            async HVoid Wait(HTask<T> task) {
                await task;
                wait.SetResult();
            }

            if (wait.IsCompleted)
                return;

            await wait;
        }
    }
}