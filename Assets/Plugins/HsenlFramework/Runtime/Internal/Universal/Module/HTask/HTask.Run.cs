using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hsenl {
    public partial struct HTask {
        // 运行在线程池中(多线程)
        public static async HTask Run(Action action, bool returnMainThread = true) {
            await SwitchToThreadPoolAwaitable.Create();
            if (returnMainThread) {
                try {
                    action.Invoke();
                }
                finally {
                    await SwitchToMainThreadPoolAwaitable.Create();
                }
            }
            else {
                action.Invoke();
            }
        }
    }
}