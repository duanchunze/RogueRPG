﻿using System.Collections.Generic;

namespace Hsenl {
    public class SortedMultiDictionary<T, M, N> : SortedDictionary<T, SortedDictionary<M, N>> {
        public N this[T t, M m] {
            get => this[t][m];
            set {
                this.TryGetValue(t, out var kSet);
                if (kSet == null) {
                    kSet = new SortedDictionary<M, N>();
                    this[t] = kSet;
                }

                kSet[m] = value;
            }
        }

        public bool TryGetDict(T t, out SortedDictionary<M, N> k) {
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
                kSet = new SortedDictionary<M, N>();
                this[t] = kSet;
            }

            kSet.Add(m, n);
        }

        public bool Remove(T t, M m) {
            this.TryGetValue(t, out var dic);
            if (dic == null || !dic.Remove(m)) {
                return false;
            }

            if (dic.Count == 0) {
                this.Remove(t);
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

            return dic.ContainsValue(n);
        }
    }
}