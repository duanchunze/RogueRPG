using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

namespace Hsenl {
    [Serializable]
    public class ShadowFunctionManager : Singleton<ShadowFunctionManager> {
        [ShowInInspector, ReadOnly, HideLabel]
        private readonly Dictionary<uint, SortedDictionary<long, Delegate>> _dictionary = new();

        [ShowInInspector, ReadOnly, HideLabel]
        private readonly List<object> _shadowInstances = new();

        protected override void OnSingleUnregister() {
            this.Clear();
        }

        public void Register(uint hashcode, string assemblyName, int priority, Delegate del) {
            var sourcePriority = ((long)priority << 32) | (long)assemblyName.GetHashCode();
            if (!this._dictionary.TryGetValue(hashcode, out var sortedDict)) {
                sortedDict = new(new ShadowFunctionPriorityComparer()) { { sourcePriority, del } };
                this._dictionary.Add(hashcode, sortedDict);
                return;
            }

            if (sortedDict.ContainsKey(sourcePriority)) {
                throw new Exception($"Shadow function already exist: '{assemblyName}' '{priority}' '{del}'");
            }

            sortedDict.Add(sourcePriority, del);
        }

        public bool Unregister(uint hashcode) {
            return this._dictionary.Remove(hashcode);
        }

        public bool Unregister(uint hashcode, string source, int priority) {
            if (this._dictionary.TryGetValue(hashcode, out var dict)) {
                var sourcePriority = ((long)priority << 32) | (long)source.GetHashCode();
                return dict.Remove(sourcePriority);
            }

            return false;
        }

        public void Clear() {
            this._dictionary.Clear();
        }

        public bool GetFunctions(uint hashcode, out SortedDictionary<long, Delegate> dels) {
            return this._dictionary.TryGetValue(hashcode, out dels!);
        }

        [OnEventSystemInitialized]
        private static void RegisterShadowFunctions() {
            if (!SingletonManager.IsDisposed<ShadowFunctionManager>()) {
                SingletonManager.Unregister<ShadowFunctionManager>();
            }

            SingletonManager.Register<ShadowFunctionManager>();

            Register(EventSystem.GetTypesOfAttribute(typeof(ShadowFunctionAttribute)));
        }

        public static void Register(IReadOnlyList<Type> types) {
            for (int i = 0, len = types.Count; i < len; i++) {
                var type = types[i];
                var attr = type.GetCustomAttribute<ShadowFunctionAttribute>();
                if (attr.targetType != null) {
                    if (type.IsAbstract && type.IsSealed) {
                        // 静态类型
                        var method = type.GetMethod("Register", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                        if (method == null) throw new Exception($"ShadowType '{type.FullName}' register fail, cant find 'Register' method");
                        method.Invoke(null, null);
                    }
                    else {
                        // 非静态类型的话, 需要实例化, 并缓存起来, 然后调用Register
                        var o = Activator.CreateInstance(type);
                        var method = type.GetMethod("Register", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                        if (method == null) throw new Exception($"ShadowType '{type.FullName}' register fail, cant find 'Register' method");
                        Instance._shadowInstances.Add(o);
                        method.Invoke(o, null);
                    }
                }
            }
        }

        public class ShadowFunctionPriorityComparer : IComparer<long> {
            public int Compare(long x, long y) {
                var xPriority = x >> 32;
                var yPriority = y >> 32;
                return xPriority.CompareTo(yPriority);
            }
        }
    }
}