using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Hsenl {
    public struct SwitchToMainThreadPoolAwaitable {
        public static SwitchToMainThreadPoolAwaitable Create() => default;
        
        public Awaiter GetAwaiter() => default;

        public struct Awaiter : ICriticalNotifyCompletion {
            public bool IsCompleted => false;

            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                Framework.Instance.ThreadSynchronizationContext.Post(continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                Framework.Instance.ThreadSynchronizationContext.Post(continuation);
            }
        }
    }
    
    public struct SwitchToThreadPoolAwaitable {
        public static SwitchToThreadPoolAwaitable Create() => default;
        
        public Awaiter GetAwaiter() => default;

        public struct Awaiter : ICriticalNotifyCompletion {
            public bool IsCompleted => false;

            public void GetResult() { }

            public void OnCompleted(Action continuation) {
                ThreadPool.QueueUserWorkItem(SwitchCompletedCallback, continuation);
            }

            public void UnsafeOnCompleted(Action continuation) {
                ThreadPool.UnsafeQueueUserWorkItem(SwitchCompletedCallback, continuation);
            }

            private static void SwitchCompletedCallback(object state) {
                ((Action)state).Invoke();
            }
        }
    }
}