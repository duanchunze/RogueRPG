using System;

namespace Hsenl {
    public static class ObjectPool {
        public static T Fetch<T>() where T : class, new() => ObjectPoolManager.Instance.Fetch<T>();
        public static object Fetch(Type type) => ObjectPoolManager.Instance.Fetch(type);
        public static void Recycle(object obj) => ObjectPoolManager.Instance.Recycle(obj);
        public static void Clear() => ObjectPoolManager.Instance.Clear();
    }
}