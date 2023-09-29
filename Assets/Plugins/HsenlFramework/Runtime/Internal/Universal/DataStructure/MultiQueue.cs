using System.Collections.Generic;

namespace Hsenl {
    public class MultiQueue<TKey, TValue> : Dictionary<TKey, Queue<TValue>> {
        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new Queue<TValue> this[TKey t] => this.TryGetValue(t, out var queue) ? queue : null;

        public bool Contains(TKey key, TValue value) {
            this.TryGetValue(key, out var queue);
            if (queue == null) {
                return false;
            }

            return queue.Contains(value);
        }

        public void Enqueue(TKey key, TValue value) {
            this.TryGetValue(key, out var queue);
            if (queue == null) {
                queue = new Queue<TValue>();
                base[key] = queue;
            }

            queue.Enqueue(value);
        }

        public TValue Dequeue(TKey key) {
            this.TryGetValue(key, out var queue);
            if (queue == null) {
                return default;
            }

            var result = queue.Dequeue();

            if (queue.Count == 0) {
                this.Remove(key);
            }

            return result;
        }
    }
}