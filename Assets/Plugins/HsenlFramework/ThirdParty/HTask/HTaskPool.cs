using System;
using System.Collections.Generic;

namespace Hsenl {
    public static class HTaskPool {
        private static readonly Dictionary<Type, Queue<IHTask>> _pool = new();

        internal static T Rent<T>() where T : IHTask, new() {
            var type = typeof(T);
            if (!_pool.TryGetValue(type, out var queue)) {
                return new T();
            }

            var ret = (T)queue.Dequeue();
            if (queue.Count == 0) {
                _pool.Remove(type);
            }

            return ret;
        }

        internal static IHTask Rent(Type type) {
            if (!_pool.TryGetValue(type, out var queue)) {
                return (IHTask)Activator.CreateInstance(type)!;
            }

            var ret = queue.Dequeue();
            if (queue.Count == 0) {
                _pool.Remove(type);
            }

            return ret;
        }

        internal static void Return(IHTask task) {
            Console.WriteLine("return task");
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

    public static class HTaskPool<T> {
        private static readonly Dictionary<Type, Queue<IHTask<T>>> _pool = new();

        internal static TH Rent<TH>() where TH : IHTask<T>, new() {
            var type = typeof(TH);
            if (!_pool.TryGetValue(type, out var queue)) {
                return new TH();
            }

            var ret = (TH)queue.Dequeue();
            if (queue.Count == 0) {
                _pool.Remove(type);
            }

            return ret;
        }

        internal static IHTask<T> Rent(Type type) {
            if (!_pool.TryGetValue(type, out var queue)) {
                return (IHTask<T>)Activator.CreateInstance(type)!;
            }

            var ret = queue.Dequeue();
            if (queue.Count == 0) {
                _pool.Remove(type);
            }

            return ret;
        }

        internal static void Return(IHTask<T> task) {
            Console.WriteLine("return task");
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