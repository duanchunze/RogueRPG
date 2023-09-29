using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class ObjectPoolManager : Singleton<ObjectPoolManager> {
        [ShowInInspector, ReadOnly]
        private Dictionary<Type, Queue<object>> _pool = new();

        public T Fetch<T>() where T : class, new() {
            return this.Fetch(typeof(T)) as T;
        }

        public object Fetch(Type type) {
            object result = null;
            if (this._pool.TryGetValue(type, out var queue)) {
                if (queue.Count != 0) {
                    result = queue.Dequeue();
                }
            }

            result ??= Activator.CreateInstance(type);
            return result;
        }

        public void Recycle(object obj) {
            if (obj == null) {
                return;
            }

            var type = obj.GetType();
            if (!this._pool.TryGetValue(type, out var queue)) {
                queue = new Queue<object>();
                this._pool.Add(type, queue);
            }

            // 一种对象最大为1000个
            if (queue.Count > 1000) {
                return;
            }

            queue.Enqueue(obj);
        }

        public void Clear() {
            this._pool.Clear();
        }

        protected override void Dispose() {
            base.Dispose();

            this.Clear();
        }
    }
}