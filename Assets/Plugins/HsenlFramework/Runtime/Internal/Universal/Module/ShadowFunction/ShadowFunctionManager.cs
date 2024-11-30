using System;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using Sirenix.OdinInspector;

// ReSharper disable HeuristicUnreachableCode
#pragma warning disable CS0162 // 检测到不可到达的代码
#endif

namespace Hsenl {
    /*
     * 影子系统可以在多线程中执行
     *
     * 注意: ShadowFunctionManager中的所有的函数, 都不应该由用户调用, 应该由影子系统自行调用.
     */
    [Serializable]
    public class ShadowFunctionManager : Singleton<ShadowFunctionManager> {
        // 每个源函数的唯一编号就等于其在列表中的索引, 获取速度比字典快7倍左右, 50w次执行快20ms左右(参考: 空转1ms, list 4ms, dict 24ms)
        // todo 后续测试运行稳定后, 可以移除该参数, 并永久使用index作为key
        private const bool UseIndex = true;
#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly Dictionary<int, List<DelegateWrap>> _dictionary = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly List<List<DelegateWrap>> _list = new();

        private readonly Dictionary<int, string> _singleSourceFuncs = new();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly, HideLabel]
#endif
        private readonly List<object> _shadowInstances = new();

        private readonly object _locker = new();

        protected override void OnUnregister() { }

        public void Register(int hashcode, string assemblyName, string typeFullName, int priority, Delegate del) {
            var id = HashCode.Combine(hashcode, assemblyName, typeFullName, priority);
            List<DelegateWrap> list;
            if (!UseIndex) {
                if (!this._dictionary.TryGetValue(hashcode, out list)) {
                    list = new List<DelegateWrap>() { new(priority, del, id) };
                    this._dictionary.Add(hashcode, list);
                    return;
                }
            }
            else {
                if (hashcode > 100000)
                    throw new Exception("这个数有点过大, 可能是没和源生成器保持一致, 前者使用的是Hashcode模式, 而这里却使用了Index模式");

                if (hashcode + 1 > this._list.Count) {
                    for (int i = this._list.Count; i < hashcode + 1; i++) {
                        this._list.Add(null);
                    }
                }

                list = this._list[hashcode];
                if (list == null) {
                    list = new List<DelegateWrap>() { new(priority, del, id) };
                    this._list[hashcode] = list;
                    return;
                }
            }

            if (this._singleSourceFuncs.TryGetValue(hashcode, out var uniqueName)) {
                throw new Exception($"This source function does not allow multiple shadow functions! '{uniqueName}'");
            }

            foreach (var wrap in list) {
                if (wrap.Id == id) {
                    // 应该不会出现完全重复的影子函数, 编译器都过不去
                    throw new Exception("Should not appear!");
                }
            }

            list.Add(new DelegateWrap(priority, del, id));
            list.Sort((wrap1, wrap2) => wrap1.Key.CompareTo(wrap2.Key));
        }

        public bool Unregister(int hashcode) {
            if (!UseIndex) {
                return this._dictionary.Remove(hashcode);
            }
            else {
                if (hashcode >= this._list.Count)
                    return false;
                this._list[hashcode] = null;
                return true;
            }
        }

        public bool Unregister(int hashcode, string assemblyName, string typeFullName, int priority) {
            var id = HashCode.Combine(hashcode, assemblyName, typeFullName, priority);
            List<DelegateWrap> list;
            if (!UseIndex) {
                this._dictionary.TryGetValue(hashcode, out list);
            }
            else {
                if (hashcode >= this._list.Count)
                    return false;
                list = this._list[hashcode];
            }

            if (list == null) return false;

            for (int i = 0, len = list.Count; i < len; i++) {
                var wrap = list[i];
                if (wrap.Id == id) {
                    list.RemoveAt(i);
                    break;
                }
            }

            if (list.Count == 0) {
                this.Unregister(hashcode);
            }

            return false;
        }

        public bool GetFunctions(int hashcode, out List<DelegateWrap> dels) {
            lock (this._locker) {
                if (!UseIndex) {
                    return this._dictionary.TryGetValue(hashcode, out dels!);
                }
                else {
                    if (hashcode >= this._list.Count) {
                        dels = null;
                        return false;
                    }

                    dels = this._list[hashcode];
                    return dels != null;
                }
            }
        }

        public bool GetFunction(int hashcode, out Delegate del) {
            lock (this._locker) {
                if (!UseIndex) {
                    if (this._dictionary.TryGetValue(hashcode, out var dels)) {
                        del = dels![0].Value;
                        return true;
                    }

                    del = null;
                    return false;
                }
                else {
                    if (hashcode >= this._list.Count) {
                        del = null;
                        return false;
                    }

                    var dels = this._list[hashcode];
                    if (dels == null) {
                        del = null;
                        return false;
                    }

                    del = dels[0].Value;
                    return del != null;
                }
            }
        }

        [OnEventSystemInitialized, OnEventSystemReload]
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

                    var typeName = $"{type.Assembly.GetName().Name}.SFS_DC_SourceFunctionLookupTable_Single";
                    var singleSourceLookupTableType = type.Assembly.GetType(typeName);
                    if (singleSourceLookupTableType != null) {
                        var o = Activator.CreateInstance(singleSourceLookupTableType);
                        if (singleSourceLookupTableType.GetField("lookupTable", BindingFlags.Public | BindingFlags.Instance)?.GetValue(o) is
                            List<(int hashcode, string uniqueName)> list) {
                            foreach (var tuple in list) {
                                Instance._singleSourceFuncs.Add(tuple.hashcode, tuple.uniqueName);
                            }
                        }
                    }
                    else {
                        Log.Error($"Can not find AutoCeneratorScript '{typeName}'");
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
            private readonly int _id;

            public int Key => this._priority;
            public Delegate Value => this._delegate;
            public int Id => this._id;

            public DelegateWrap(int priority, Delegate @delegate, int id) {
                this._priority = priority;
                this._delegate = @delegate;
                this._id = id;
            }
        }
    }
}