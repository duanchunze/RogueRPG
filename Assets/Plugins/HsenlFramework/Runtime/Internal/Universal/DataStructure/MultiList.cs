using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable(GenerateType.Collection)]
    public partial class MultiList<T, K> : Dictionary<T, List<K>> where T : notnull {
        public void Add(T t, K k) {
            this.TryGetValue(t, out var list);
            if (list == null) {
                list = new List<K>();
                base[t] = list;
            }

            list.Add(k);
        }

        public bool Remove(T t, K k) {
            this.TryGetValue(t, out var list);
            if (list == null) {
                return false;
            }

            if (!list.Remove(k)) {
                return false;
            }

            if (list.Count == 0) {
                this.Remove(t);
            }

            return true;
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t) {
            this.TryGetValue(t, out var list);
            if (list == null) {
                return Array.Empty<K>();
            }

            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [NotNull]
        public new List<K> this[T t] {
            get {
                this.TryGetValue(t, out var list);
                return list;
            }
        }

        public K GetOne(T t) {
            this.TryGetValue(t, out var list);
            if (list != null && list.Count > 0) {
                return list[0];
            }

            return default(K);
        }

        public bool Contains(T t, K k) {
            this.TryGetValue(t, out var list);
            if (list == null) {
                return false;
            }

            return list.Contains(k);
        }
    }
}