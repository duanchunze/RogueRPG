using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class ShadowFunctionManager : Singleton<ShadowFunctionManager> {
        [ShowInInspector, ReadOnly, HideLabel]
        private readonly Dictionary<int, Delegate> _shadowFunctions = new();

        protected override void Dispose() {
            this._shadowFunctions.Clear();
        }

        public void Register(int hashcode, Delegate del) {
            this._shadowFunctions[hashcode] = del;
        }

        public void Unregister(int hashcode) {
            this._shadowFunctions.Remove(hashcode);
        }

        public bool GetFunction(int hashcode, out Delegate dl) {
            return this._shadowFunctions.TryGetValue(hashcode, out dl);
        }

        [OnEventSystemInitialized]
        private static void RegisterShadowFunctions() {
            if (!SingletonManager.IsDisposed<ShadowFunctionManager>()) {
                SingletonManager.Unregister<ShadowFunctionManager>();
            }

            SingletonManager.Register<ShadowFunctionManager>();

            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(ShadowFunctionAttribute))) {
                var attr = type.GetCustomAttribute<ShadowFunctionAttribute>();
                if (attr.targetType != null) {
                    type.Invoke("Register", null, null);
                }
            }
        }
    }
}