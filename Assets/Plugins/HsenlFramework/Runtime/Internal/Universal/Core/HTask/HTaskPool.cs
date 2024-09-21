using System;
using System.Collections.Generic;

namespace Hsenl {
    public static class HTaskPool {
        private static readonly Dictionary<Type, Queue<IHTaskBody>> _pool = new();

        internal static TBody Rent<TBody>() where TBody : IHTaskBody, new() {
            var type = typeof(TBody);
            if (!_pool.TryGetValue(type, out var queue)) {
                return new TBody();
            }

            if (queue.Count == 0)
                return new TBody();

            var ret = (TBody)queue.Dequeue();
            if (queue.Count == 0) {
                // _pool.Remove(type);
            }

            return ret;
        }

        internal static IHTaskBody Rent(Type type) {
            if (!_pool.TryGetValue(type, out var queue)) {
                return (IHTaskBody)Activator.CreateInstance(type)!;
            }

            if (queue.Count == 0)
                return (IHTaskBody)Activator.CreateInstance(type)!;

            var ret = queue.Dequeue();

            return ret;
        }

        internal static void Return(IHTaskBody taskBody) {
            taskBody.Dispose();
            var type = taskBody.GetType();
            if (!_pool.TryGetValue(type, out var queue)) {
                queue = new();
                _pool[type] = queue;
            }

            if (queue.Count < 1000)
                queue.Enqueue(taskBody);
        }

        public static void Clear() {
            _pool.Clear();
        }
    }

    public static class HTaskPool<T> {
        private static readonly Dictionary<Type, Queue<IHTaskBody<T>>> _pool = new();

        internal static TBody Rent<TBody>() where TBody : IHTaskBody<T>, new() {
            var type = typeof(TBody);
            if (!_pool.TryGetValue(type, out var queue)) {
                return new TBody();
            }

            if (queue.Count == 0)
                return new TBody();

            var ret = (TBody)queue.Dequeue();
            if (queue.Count == 0) {
                // _pool.Remove(type);
            }

            return ret;
        }

        internal static IHTaskBody<T> Rent(Type type) {
            if (!_pool.TryGetValue(type, out var queue)) {
                return (IHTaskBody<T>)Activator.CreateInstance(type)!;
            }

            if (queue.Count == 0)
                return (IHTaskBody<T>)Activator.CreateInstance(type)!;

            var ret = queue.Dequeue();

            return ret;
        }

        internal static void Return(IHTaskBody<T> task) {
            task.Dispose();
            var type = task.GetType();
            if (!_pool.TryGetValue(type, out var queue)) {
                queue = new();
                _pool[type] = queue;
            }

            if (queue.Count < 1000)
                queue.Enqueue(task);
        }

        public static void Clear() {
            _pool.Clear();
        }
    }
}