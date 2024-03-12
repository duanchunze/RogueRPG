using System;
using System.Collections.Generic;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // 该池针对的是dotnet object
    [Serializable]
    public class ObjectPoolManager : Singleton<ObjectPoolManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private Dictionary<Type, Queue<object>> _pool = new();

        public T Rent<T>() where T : class {
            return this.Rent(typeof(T)) as T;
        }

        public object Rent(Type type) {
            object result = null;
            if (this._pool.TryGetValue(type, out var queue)) {
                if (queue.Count != 0) {
                    result = queue.Dequeue();
                }
            }

            result ??= Activator.CreateInstance(type);
            return result;
        }

        public void Return(object obj) {
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
                Log.Warning($"object pool return exceeds the upper limit 1000 '{obj}'");
                return;
            }

            queue.Enqueue(obj);
        }

        public void Clear() {
            this._pool.Clear();
        }

        protected override void OnSingleUnregister() {
            this.Clear();
        }
    }
}