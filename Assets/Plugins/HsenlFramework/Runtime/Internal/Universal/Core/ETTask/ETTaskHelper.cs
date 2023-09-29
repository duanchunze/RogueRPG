using System.Collections.Generic;

namespace Hsenl {
    public static class ETTaskHelper {
        public static bool IsCancel(this ETCancellationToken self) {
            if (self == null) {
                return false;
            }

            return self.IsDispose();
        }

        // private class CoroutineBlocker
        // {
        //     private int count;
        //
        //     private ETTask tcs;
        //
        //     public CoroutineBlocker(int count)
        //     {
        //         this.count = count;
        //     }
        //
        //     // 放出9个人执行任务，每个人执行完毕后，都会调用该方法
        //     // 在方法中，计数，看还剩几个没执行完的
        //     // 如果都执行完了，就 SetResult
        //     public async ETTask WaitAsync(bool isTrue = false)
        //     {
        //         --this.count;
        //
        //         if (this.count < 0)
        //         {
        //             return;
        //         }
        //
        //         if (this.count == 0)
        //         {
        //             if (isTrue)
        //             {
        //                 return;
        //             }
        //
        //             ETTask t = this.tcs;
        //             this.tcs = null;
        //             t.SetResult();
        //
        //             return;
        //         }
        //
        //         if (this.count > 0)
        //         {
        //             if (isTrue)
        //             {
        //                 this.tcs = ETTask.Create(true);
        //                 await tcs;
        //             }
        //         }
        //     }
        // }

        // public static async ETTask<bool> WaitAny<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Length == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);
        //
        //     foreach (ETTask<T> task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask<T> task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     if (cancellationToken == null)
        //     {
        //         return true;
        //     }
        //
        //     return !cancellationToken.IsCancel();
        // }
        //
        // public static async ETTask<bool> WaitAny(ETTask[] tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Length == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(2);
        //
        //     foreach (ETTask task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     if (cancellationToken == null)
        //     {
        //         return true;
        //     }
        //
        //     return !cancellationToken.IsCancel();
        // }

        // public static async ETTask<bool> WaitAll<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Length == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);
        //
        //     foreach (ETTask<T> task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask<T> task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     if (cancellationToken == null)
        //     {
        //         return true;
        //     }
        //
        //     return !cancellationToken.IsCancel();
        // }
        //
        // public static async ETTask<bool> WaitAll<T>(List<ETTask<T>> tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Count == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);
        //
        //     foreach (ETTask<T> task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask<T> task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     if (cancellationToken == null)
        //     {
        //         return true;
        //     }
        //
        //     return !cancellationToken.IsCancel();
        // }
        //
        // public static async ETTask<bool> WaitAll(ETTask[] tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Length == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Length + 1);
        //
        //     foreach (ETTask task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     if (cancellationToken == null)
        //     {
        //         return true;
        //     }
        //
        //     return !cancellationToken.IsCancel();
        // }
        //
        // public static async ETTask<bool> WaitAll(List<ETTask> tasks, ETCancellationToken cancellationToken = null)
        // {
        //     if (tasks.Count == 0)
        //     {
        //         return false;
        //     }
        //
        //     CoroutineBlocker coroutineBlocker = new CoroutineBlocker(tasks.Count + 1);
        //
        //     foreach (ETTask task in tasks)
        //     {
        //         RunOneTask(task).Coroutine();
        //     }
        //
        //     async ETVoid RunOneTask(ETTask task)
        //     {
        //         await task;
        //         await coroutineBlocker.WaitAsync();
        //     }
        //
        //     await coroutineBlocker.WaitAsync(true);
        //
        //     return !cancellationToken.IsCancel();
        // }

        public static async ETTask<bool> WaitAny<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Length == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = 1;

            for (var i = 0; i < tasks.Length; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask<T> task) {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }

        public static async ETTask<bool> WaitAny(ETTask[] tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Length == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = 1;

            for (var i = 0; i < tasks.Length; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask task) {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }

        public static async ETTask<bool> WaitAll<T>(ETTask<T>[] tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Length == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = tasks.Length;

            for (var i = 0; i < tasks.Length; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask<T> task) {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }

        public static async ETTask<bool> WaitAll<T>(List<ETTask<T>> tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Count == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = tasks.Count;

            for (var i = 0; i < tasks.Count; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask<T> task) {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }

        public static async ETTask<bool> WaitAll(ETTask[] tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Length == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = tasks.Length;

            for (var i = 0; i < tasks.Length; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask task) {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }

        public static async ETTask<bool> WaitAll(List<ETTask> tasks, ETCancellationToken cancellationToken = null) {
            if (tasks.Count == 0) {
                return false;
            }

            var tcs = ETTask.Create(true);

            var count = tasks.Count;

            for (var i = 0; i < tasks.Count; i++) {
                Run(tasks[i]);
            }

            async void Run(ETTask task) // 原框架使用的是ETVoid，后续注意
            {
                await task;
                count--;
                if (count == 0) {
                    tcs.SetResult();
                }
            }

            if (count > 0) {
                await tcs;
            }

            return !cancellationToken?.IsCancel() ?? true;
        }
    }
}