using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hsenl {
    public interface IUnitySingleton {
        void Register();
        void Unregister();
    }

    public static class UnitySingletonManager {
        private static readonly Dictionary<Type, IUnitySingleton> _singletons = new();

        public static IReadOnlyDictionary<Type, IUnitySingleton> GetSingletons() => _singletons;

        public static T Register<T>(T singleton) where T : UnitySingleton<T> {
            var singletonType = typeof(T);
            if (singleton == null) throw new NullReferenceException($"unity singleton '{singletonType}' is null");
            if (_singletons.ContainsKey(singletonType)) {
                throw new Exception($"already exist unity singleton: {singletonType.Name}");
            }

            _singletons.Add(singletonType, singleton);
            ((IUnitySingleton)singleton).Register();
            return singleton;
        }

        public static void Unregister<T>() where T : UnitySingleton<T> {
            var singletonType = typeof(T);
            if (!_singletons.TryGetValue(singletonType, out var singleton)) {
                return;
            }

            _singletons.Remove(singletonType);
            singleton.Unregister();
        }

        public static bool IsDisposed<T>() where T : UnitySingleton<T> {
            return !_singletons.ContainsKey(typeof(T));
        }
    }

    public abstract class UnitySingleton<T> : MonoBehaviour, IUnitySingleton where T : UnitySingleton<T> {
        public static T Instance { get; private set; }

        protected virtual void Awake() {
            if (!UnitySingletonManager.IsDisposed<T>()) {
                UnitySingletonManager.Unregister<T>();
            }
            
            UnitySingletonManager.Register((T)this);
        }

        void IUnitySingleton.Register() {
            if (Instance != null) {
                throw new Exception($"unity singleton register twice! {typeof(T).Name}");
            }

            Instance = (T)this;
        }

        void IUnitySingleton.Unregister() {
            if (Instance != this) {
                throw new Exception($"unity singleton unregister error! {typeof(T).Name}");
            }

            Instance = null;
        }
    }
}