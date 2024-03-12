using System;

namespace Hsenl {
    public static class ObjectPool {
        public static T Rent<T>() where T : class => ObjectPoolManager.Instance.Rent<T>();
        public static object Rent(Type type) => ObjectPoolManager.Instance.Rent(type);
        public static void Return(object obj) => ObjectPoolManager.Instance.Return(obj);
        public static void Clear() => ObjectPoolManager.Instance.Clear();
    }
}