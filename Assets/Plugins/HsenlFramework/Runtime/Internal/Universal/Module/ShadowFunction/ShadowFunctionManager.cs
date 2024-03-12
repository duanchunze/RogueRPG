using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    /*
     * 影子系统可以在多线程中执行, 即便他什么锁都没加, 因为即便当重载时, 存在有多线程正在GetFunctions, 他获取的func依然是有效的, 只不过是旧的, 而当下次他再GetFunctions的时候, 获取的就是
     * 新的, 在实际运行中, 这并不会造成什么影响.
     *
     * 需要注意的是, Register与Unregister并不支持多线程, 但这些函数本就不是给用户使用的, 用户想调用他们, 也比较困难.
     * 特别注意: ShadowFunctionManager中的所有的函数, 都不应该由用户调用, 应该由影子系统自行调用.
     */
    [Serializable]
    public class ShadowFunctionManager : Singleton<ShadowFunctionManager> {
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly Dictionary<uint, List<DelegateWrap>> _dictionary = new();

        private readonly Dictionary<uint, string> _singleSourceFuncs = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly List<object> _shadowInstances = new();

        protected override void OnSingleUnregister() { }

        public void Register(uint hashcode, string assemblyName, string typeFullName, int priority, Delegate del) {
            var shadowHashcode = (uint)HashCode.Combine(hashcode, assemblyName, typeFullName, priority);
            if (!this._dictionary.TryGetValue(hashcode, out var list)) {
                list = new List<DelegateWrap>() { new(priority, del, shadowHashcode) };
                this._dictionary.Add(hashcode, list);
                return;
            }

            if (this._singleSourceFuncs.TryGetValue(hashcode, out var uniqueName)) {
                throw new Exception($"This source function does not allow multiple shadow functions! '{uniqueName}'");
            }

            foreach (var wrap in list) {
                if (wrap.HashCode == shadowHashcode) {
                    // 应该不会出现完全重复的影子函数, 编译器都过不去
                    throw new Exception("Should not appear!");
                }
            }

            list.Add(new DelegateWrap(priority, del, shadowHashcode));
            list.Sort((wrap1, wrap2) => wrap1.Key.CompareTo(wrap2.Key));
        }

        public bool Unregister(uint hashcode) {
            return this._dictionary.Remove(hashcode);
        }

        public bool Unregister(uint hashcode, string assemblyName, string typeFullName, int priority) {
            var shadowHashcode = (uint)HashCode.Combine(hashcode, assemblyName, typeFullName, priority);
            if (this._dictionary.TryGetValue(hashcode, out var list)) {
                for (int i = 0, len = list.Count; i < len; i++) {
                    var wrap = list[i];
                    if (wrap.HashCode == shadowHashcode) {
                        list.RemoveAt(i);
                        break;
                    }
                }
            }

            return false;
        }

        public bool GetFunctions(uint hashcode, out List<DelegateWrap> dels) {
            return this._dictionary.TryGetValue(hashcode, out dels!);
        }

        [OnEventSystemInitialized, OnEventSystemChanged]
        private static void RegisterShadowFunctions() {
            if (!SingletonManager.IsDisposed<ShadowFunctionManager>()) {
                SingletonManager.Unregister<ShadowFunctionManager>();
            }

            SingletonManager.Register<ShadowFunctionManager>();

            PreprocessSourceFunction(EventSystem.GetTypesOfAttribute(typeof(ShadowFunctionAttribute)));
            Register(EventSystem.GetTypesOfAttribute(typeof(ShadowFunctionAttribute)));
        }

        private static void PreprocessSourceFunction(IReadOnlyList<Type> types) {
            var alreadyHandledAssembly = new HashSet<Assembly>();
            for (int i = 0, len = types.Count; i < len; i++) {
                var type = types[i];
                var attr = type.GetCustomAttribute<ShadowFunctionAttribute>();
                if (attr.TargetType == null) {
                    if (!alreadyHandledAssembly.Add(type.Assembly))
                        continue;

                    var singleSourceLookupTableType = type.Assembly.GetType($"{type.Assembly.GetName().Name}.SingleSourceFunctionLookupTable");
                    if (singleSourceLookupTableType != null) {
                        var o = Activator.CreateInstance(singleSourceLookupTableType);
                        if (singleSourceLookupTableType.GetField("lookupTable", BindingFlags.Public | BindingFlags.Instance)?.GetValue(o) is
                            List<(uint hashcode, string uniqueName)> list) {
                            foreach (var tuple in list) {
                                Instance._singleSourceFuncs.Add(tuple.hashcode, tuple.uniqueName);
                            }
                        }
                    }
                }
            }
        }

        private static void Register(IReadOnlyList<Type> types) {
            for (int i = 0, len = types.Count; i < len; i++) {
                var type = types[i];
                var attr = type.GetCustomAttribute<ShadowFunctionAttribute>();
                if (attr.TargetType != null) {
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

        public struct DelegateWrap {
            private readonly int _priority;
            private readonly Delegate _delegate;
            private readonly uint _hashcode;

            public int Key => this._priority;
            public Delegate Value => this._delegate;
            public uint HashCode => this._hashcode;

            public DelegateWrap(int priority, Delegate @delegate, uint hashcode) {
                this._priority = priority;
                this._delegate = @delegate;
                this._hashcode = hashcode;
            }
        }
    }
}