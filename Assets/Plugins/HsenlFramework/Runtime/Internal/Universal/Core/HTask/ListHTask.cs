using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Hsenl {
    public class ListHTask<T> : List<T> where T : IHTask {
        private static readonly Pool _instance = new();

        public static ListHTask<T> Create() {
            ListHTask<T> list = null;
            if (_instance.pool.Count != 0) {
                _instance.pool.TryDequeue(out list);
            }

            list ??= new ListHTask<T>();
            return list;
        }

        public static void Return(ListHTask<T> list) {
            if (list == null) {
                return;
            }
            
            list.Clear();

            // 一种对象最大为1000个
            if (_instance.pool.Count > 1000) {
                Log.Warning($"ListHTask<> pool return exceeds the upper limit 1000 '{nameof(list)}'");
                return;
            }

            _instance.pool.Enqueue(list);
        }

        public void Dispose() {
            Return(this);
        }

        private class Pool {
            public readonly ConcurrentQueue<ListHTask<T>> pool = new();
        }
    }
}