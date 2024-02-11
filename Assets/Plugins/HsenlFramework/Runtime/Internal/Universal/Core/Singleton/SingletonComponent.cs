using System;
using UnityEngine;

namespace Hsenl {
    [FrameworkMember]
    public class SingletonComponent<T> : Component, ISingleton where T : SingletonComponent<T>, new() {
        public static T Instance { get; private set; }

        void ISingleton.Register() {
            if (Instance != null) {
                throw new Exception($"singleton register twice! {typeof(T).Name}");
            }

            Instance = (T)this;
        }

        void ISingleton.Unregister() {
            if (Instance != this) {
                throw new Exception($"singleton unregister error! {typeof(T).Name}");
            }

            var t = Instance;
            Instance = null;
            t.Dispose();
        }

        internal override void OnAwakeInternal() {
            if (!SingletonManager.IsDisposed<T>()) {
                SingletonManager.Unregister<T>();
            }

            SingletonManager.Register((T)this);
        }

        protected virtual void Dispose() { }
    }
}