using System.Collections.Concurrent;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.Collection)]
    public partial class ConcurrentMultiDictionary<T, M, N> : ConcurrentDictionary<T, ConcurrentDictionary<M, N>> where T : notnull {
        public N this[T t, M m] {
            get => this[t][m];
            set {
                this.TryGetValue(t, out var kSet);
                if (kSet == null) {
                    kSet = new ConcurrentDictionary<M, N>();
                    this[t] = kSet;
                }

                kSet[m] = value;
            }
        }

        public bool TryGetDict(T t, out ConcurrentDictionary<M, N> k) {
            return this.TryGetValue(t, out k);
        }

        public bool TryGetValue(T t, M m, out N n) {
            n = default;

            if (!this.TryGetValue(t, out var dic)) {
                return false;
            }

            return dic.TryGetValue(m, out n);
        }

        public void Add(T t, M m, N n) {
            this.TryGetValue(t, out var kSet);
            if (kSet == null) {
                kSet = new ConcurrentDictionary<M, N>();
                this[t] = kSet;
            }

            kSet.TryAdd(m, n);
        }

        public bool Remove(T t, M m) {
            this.TryGetValue(t, out var dic);
            if (dic == null || !dic.TryRemove(m, out _)) {
                return false;
            }

            if (dic.Count == 0) {
                this.TryRemove(t, out _);
            }

            return true;
        }

        public bool ContainSubKey(T t, M m) {
            this.TryGetValue(t, out var dic);
            if (dic == null) {
                return false;
            }

            return dic.ContainsKey(m);
        }

        public bool ContainValue(T t, M m, N n) {
            this.TryGetValue(t, out var dic);
            if (dic == null) {
                return false;
            }

            if (!dic.ContainsKey(m)) {
                return false;
            }

            foreach (var kv in dic) {
                if (kv.Value.Equals(n))
                    return true;
            }

            return false;
        }
    }
}