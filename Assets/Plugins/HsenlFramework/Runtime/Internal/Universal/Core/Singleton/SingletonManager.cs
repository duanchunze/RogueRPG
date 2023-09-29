using System;
using System.Collections.Generic;
using System.Linq;

namespace Hsenl {
    public static class SingletonManager {
        private static readonly Dictionary<Type, ISingleton> _singletonTypes = new();

        public static IReadOnlyDictionary<Type, ISingleton> GetSingletons() => _singletonTypes;

        public static T Register<T>() where T : ISingleton, new() {
            var singletonType = typeof(T);
            if (_singletonTypes.ContainsKey(singletonType)) {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            var singleton = new T();
            _singletonTypes.Add(singletonType, singleton);
            singleton.Register();
            return singleton;
        }

        public static T Register<T>(T singleton) where T : ISingleton, new() {
            if (singleton == null)
                throw new ArgumentNullException();

            var singletonType = typeof(T);
            if (_singletonTypes.ContainsKey(singletonType)) {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            _singletonTypes.Add(singletonType, singleton);
            singleton.Register();
            return singleton;
        }

        // 可以由内部创建，也可以从外部指定已经实例好的单例
        public static T Register<T>(ref T singleton) where T : ISingleton, new() {
            var singletonType = typeof(T);
            if (_singletonTypes.ContainsKey(singletonType)) {
                throw new Exception($"already exist singleton: {singletonType.Name}");
            }

            singleton ??= new T();
            _singletonTypes.Add(singletonType, singleton);
            singleton.Register();
            return singleton;
        }

        public static void Unregister<T>() where T : ISingleton, new() {
            var singletonType = typeof(T);
            Unregister(singletonType);
        }

        public static void Unregister(Type singletonType) {
            if (!_singletonTypes.TryGetValue(singletonType, out var singleton)) {
                return;
            }

            _singletonTypes.Remove(singletonType);
            singleton.Unregister();
        }

        public static void UnregisterAll() {
            foreach (var singletonType in _singletonTypes.Keys.ToArray()) {
                Unregister(singletonType);
            }
        }

        public static bool IsDisposed<T>() where T : ISingleton, new() {
            return IsDisposed(typeof(T));
        }

        public static bool IsDisposed(Type singletonType) {
            return !_singletonTypes.ContainsKey(singletonType);
        }
    }
}