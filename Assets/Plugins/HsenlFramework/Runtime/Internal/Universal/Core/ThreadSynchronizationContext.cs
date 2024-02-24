using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Hsenl {
    public class ThreadSynchronizationContext : SynchronizationContext {
        private readonly int _threadId = Thread.CurrentThread.ManagedThreadId;
        private readonly ConcurrentQueue<Action> _actionQueue = new();

        public void Update() {
            while (true) {
                if (!this._actionQueue.TryDequeue(out var action)) {
                    return;
                }

                try {
                    action.Invoke();
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public override void Post(SendOrPostCallback d, object state) {
            this.Post(() => d(state));
        }

        public void Post(Action action) {
            if (action == null)
                return;

            if (Thread.CurrentThread.ManagedThreadId != this._threadId) {
                try {
                    this._actionQueue.Enqueue(action);
                }
                catch (Exception e) {
                    Log.Error(e);
                }

                return;
            }

            this._actionQueue.Enqueue(action);
        }
    }
}