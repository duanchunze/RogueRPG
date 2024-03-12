using System.Collections.Concurrent;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.Collection)]
    public partial class ConcurrentMultiQueue<TKey, TValue> : ConcurrentDictionary<TKey, ConcurrentQueue<TValue>> where TKey : notnull {
        public new ConcurrentQueue<TValue> this[TKey t] => this.TryGetValue(t, out var queue) ? queue : null;

        public void Enqueue(TKey key, TValue value) {
            this.TryGetValue(key, out var queue);
            if (queue == null) {
                queue = new ConcurrentQueue<TValue>();
                base[key] = queue;
            }

            queue.Enqueue(value);
        }

        public TValue Dequeue(TKey key) {
            this.TryGetValue(key, out var queue);
            if (queue == null) {
                return default;
            }

            queue.TryDequeue(out var result);

            if (queue.Count == 0) {
                this.TryRemove(key, out _);
            }

            return result;
        }
    }
}