using System.Collections.Generic;

namespace Hsenl {
    public class MyHashTable<TKey, TValue> {
        private const int DefaultCapacity = 10;
        private readonly LinkedList<KeyValuePair<TKey, TValue>>[] buckets;

        public MyHashTable() {
            this.buckets = new LinkedList<KeyValuePair<TKey, TValue>>[DefaultCapacity];
        }

        private int GetBucketIndex(TKey key) {
            var hash = key.GetHashCode();
            var index = hash % this.buckets.Length;
            return index;
        }

        public void Add(TKey key, TValue value) {
            var index = this.GetBucketIndex(key);

            if (this.buckets[index] == null) {
                this.buckets[index] = new LinkedList<KeyValuePair<TKey, TValue>>();
            }

            var bucket = this.buckets[index];

            foreach (var item in bucket) {
                if (item.Key.Equals(key)) {
                    throw new System.Exception("Key already exists");
                }
            }

            bucket.AddLast(new KeyValuePair<TKey, TValue>(key, value));
        }

        public bool ContainsKey(TKey key) {
            var index = this.GetBucketIndex(key);
            var bucket = this.buckets[index];

            if (bucket != null) {
                foreach (var item in bucket) {
                    if (item.Key.Equals(key)) {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool ContainsValue(TValue value) {
            foreach (var bucket in this.buckets) {
                if (bucket != null) {
                    foreach (var item in bucket) {
                        if (item.Value.Equals(value)) {
                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}