using System;

namespace Hsenl {
    public interface ISingleton {
        void Register();
        void Unregister();
    }

    [FrameworkMember]
    public abstract class Singleton<T> : ISingleton where T : Singleton<T>, new() {
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

        protected virtual void Dispose() { }
    }
}