using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Hsenl {
    public partial class Container : IDisposable {
        private MultiList<Type, MappingInfo> _mappingInfos = new(); // 映射信息, key: basicType 包含了一个类(一般是接口或抽象类)和哪些实现类相映射, 且该映射关系的有效范围
        private Dictionary<Type, ResolveInfo> _resolveInfos = new(); // 解析信息, key: implementorType 包含了一个类的配置, 比如是否允许自动注入
        private Dictionary<Type, InjectionInfo> _injectionInfos = new(); // 注入信息, key: basicType 包含了一个类在做注入时的信息, 比如该类是否允许共享注入, 共享注入的范围

        private Dictionary<Type, object> _globalShareInstanceCache = new();
        private Dictionary<int, object> _specifiedShareInstanceCache = new();
        private Dictionary<Type, object> _localShareInstanceCache = new();

        /// <summary>
        /// 注册一个依赖注入
        /// </summary>
        /// <param name="basicType">一般是接口或是抽象类</param>
        /// <param name="implementorType">要注入的实现类型</param>
        /// <param name="specifiedType">该注入映射只在那个类中可用</param>
        public void RegisterMapping(Type basicType, Type implementorType, Type specifiedType = null) {
            if (!this._mappingInfos.TryGetValue(basicType, out var list)) {
                MappingInfo mappingInfo = new();
                mappingInfo.SetImplementorType(implementorType);
                if (specifiedType != null)
                    mappingInfo.AddSpecifiedType(specifiedType);
                list = new List<MappingInfo>() {
                    mappingInfo
                };

                this._mappingInfos.Add(basicType, list);
            }
            else {
                for (int i = 0, len = list.Count; i < len; i++) {
                    var info = list[i];
                    if (info.ImplementorType == implementorType) {
                        if (specifiedType == null)
                            return;
                        info.AddSpecifiedType(specifiedType);
                        list[i] = info;
                        return;
                    }
                }

                MappingInfo mappingInfo = new();
                mappingInfo.SetImplementorType(implementorType);
                if (specifiedType != null)
                    mappingInfo.AddSpecifiedType(specifiedType);
                list.Add(mappingInfo);
            }
        }

        /// <summary>
        /// 设置某对映射的实例化委托
        /// </summary>
        /// <param name="basicType"></param>
        /// <param name="implementorType"></param>
        /// <param name="instantiationFunc"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public void SetInstantiationFunc(Type basicType, Type implementorType, Func<(Type basicType, string memberName), object> instantiationFunc) {
            if (!this._mappingInfos.TryGetValue(basicType, out var list)) {
                throw new InvalidOperationException("Current mapping is not registed!");
            }

            for (int i = 0, len = list.Count; i < len; i++) {
                var info = list[i];
                if (info.ImplementorType == implementorType) {
                    info.InstantiationFunc = instantiationFunc;
                    list[i] = info;
                    return;
                }
            }
        }

        /// <summary>
        /// 让某个类可以被自动注入
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="Exception"></exception>
        public void AllowAutoInjection(Type type) {
            if (type.IsAbstract | type.IsInterface)
                throw new Exception("Can not set resolve info for a abstract type or interface");

            if (!this._resolveInfos.TryGetValue(type, out var info)) {
                info = new ResolveInfo { AllowFieldInjection = true };
                this._resolveInfos[type] = info;
                return;
            }

            info.AllowFieldInjection = true;
            this._resolveInfos[type] = info;
        }

        /// <summary>
        /// 注册一个共享的依赖注入.(例如字段a, 和字段b, 都依赖于同一个类型c, 假如字段ab都注册为了共享依赖, 那么无论哪个字段创建了类型c的实例, 另一个字段都会使用这个已有的实例, 而不是再重新创建一个)
        /// </summary>
        /// <param name="type"></param>
        /// <param name="specifiedShareType">指定在某个类型里, 才能共享, 如果共享模式为SpecifiedType的话, 则该值不能为空</param>
        /// <param name="shareInjectionModel">共享注入的模式, 不同模式, 共享的范围不一样</param>
        public void RegisterShareInjection(Type type, Type specifiedShareType = null,
            ShareInjectionModel shareInjectionModel = ShareInjectionModel.OnlyInstance) {
            if (!this._injectionInfos.TryGetValue(type, out var info)) {
                info = new InjectionInfo();
            }

            info.SetShareInjectionModel(shareInjectionModel);

            switch (shareInjectionModel) {
                case ShareInjectionModel.Global:
                    break;
                case ShareInjectionModel.SpecifiedType:
                    if (specifiedShareType == null)
                        throw new Exception("Under SpecifiedTypeModel, must specifying a type");

                    info.AddSpecifiedShareType(specifiedShareType);
                    break;
                case ShareInjectionModel.OnlyInstance:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shareInjectionModel), shareInjectionModel, null);
            }

            this._injectionInfos[type] = info;
        }

        public T Resolve<T>() {
            var type = typeof(T);
            return (T)this.Resolve(type);
        }

        public object Resolve(Type type) {
            if (this._mappingInfos.TryGetValue(type, out var mappingInfos)) {
                return this.InstantiationImplementor(type, null, mappingInfos[0]);
            }

            return this.Resolve_Internal(type);
        }

        private object Resolve_Internal(Type type) {
            object target = Activator.CreateInstance(type);
            this.Injection(target);
            return target;
        }

        private void Injection(object target) {
            var type = target.GetType();

            if (!this._resolveInfos.TryGetValue(type, out var resolveInfo))
                return;

            if (!resolveInfo.AllowFieldInjection)
                return;

            this._localShareInstanceCache.Clear();
            try {
                foreach (var reflectionInfo in InjectionReflectionCache.GetInjectionReflectionInfos(type)) {
                    if (!this._mappingInfos.TryGetValue(reflectionInfo.TargetType, out var mappingInfos)) {
                        continue;
                    }

                    var mappingInfo = mappingInfos[0];

                    // 因为在注入的时候, 是有指定类型限制的, 所以, 如果当前映射不支持当前的类, 则遍历所有映射, 看看有谁是支持的
                    if (!mappingInfo.ConformSpecifiedType(type)) {
                        for (int i = 0, len = mappingInfos.Count; i < len; i++) {
                            var mi = mappingInfos[i];
                            if (mi.ConformSpecifiedType(type)) {
                                mappingInfo = mi;
                                goto GOON;
                            }
                        }

                        continue;
                    }

                    GOON:
                    var implementorType = mappingInfo.ImplementorType;

                    object implementor;
                    // 如果是一个共享类型
                    if (this._injectionInfos.TryGetValue(reflectionInfo.TargetType, out var shareInfo)) {
                        switch (shareInfo.ShareInjectionModel) {
                            case ShareInjectionModel.Global: {
                                if (!this._globalShareInstanceCache.TryGetValue(implementorType, out var obj)) {
                                    // 依赖注入的类型, 已经是映射过的类了, 就不继续采用映射类了
                                    obj = this.InstantiationImplementor(reflectionInfo.TargetType, reflectionInfo.Name, mappingInfo);
                                    this._globalShareInstanceCache.Add(implementorType, obj);
                                }

                                implementor = obj;
                                break;
                            }
                            case ShareInjectionModel.SpecifiedType: {
                                if (shareInfo.ConformSpecifiedShareType(type)) {
                                    var hashcode = HashCode.Combine(type, implementorType);
                                    if (!this._specifiedShareInstanceCache.TryGetValue(hashcode, out var obj)) {
                                        obj = this.InstantiationImplementor(reflectionInfo.TargetType, reflectionInfo.Name, mappingInfo);
                                        this._specifiedShareInstanceCache.Add(hashcode, obj);
                                    }

                                    implementor = obj;
                                }
                                else {
                                    // 如果当前类型不包含在指定的共享类型中, 则还是创建新的
                                    implementor = this.InstantiationImplementor(reflectionInfo.TargetType, reflectionInfo.Name, mappingInfo);
                                }

                                break;
                            }
                            case ShareInjectionModel.OnlyInstance: {
                                if (!this._localShareInstanceCache.TryGetValue(implementorType, out var obj)) {
                                    obj = this.InstantiationImplementor(reflectionInfo.TargetType, reflectionInfo.Name, mappingInfo);
                                    this._localShareInstanceCache.Add(implementorType, obj);
                                }

                                implementor = obj;
                                break;
                            }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                    else {
                        implementor = this.InstantiationImplementor(reflectionInfo.TargetType, reflectionInfo.Name, mappingInfo);
                    }

                    reflectionInfo.SetValue(target, implementor);
                }
            }
            finally {
                this._localShareInstanceCache.Clear();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private object InstantiationImplementor(Type type, in string name, MappingInfo mappingInfo) {
            if (mappingInfo.InstantiationFunc == null)
                return this.Resolve_Internal(mappingInfo.ImplementorType);
            return mappingInfo.InstantiationFunc.Invoke((type, name));
        }

        public void StartStage() {
            this._globalShareInstanceCache.Clear();
            this._specifiedShareInstanceCache.Clear();
            this._localShareInstanceCache.Clear();
        }

        public void EndStage() {
            this._globalShareInstanceCache.Clear();
            this._specifiedShareInstanceCache.Clear();
            this._localShareInstanceCache.Clear();
        }

        public void Reset() {
            this._mappingInfos?.Clear();
            this._resolveInfos?.Clear();
            this._injectionInfos?.Clear();
            this._globalShareInstanceCache?.Clear();
            this._specifiedShareInstanceCache?.Clear();
            this._localShareInstanceCache?.Clear();
        }

        public void Dispose() {
            this._mappingInfos?.Clear();
            this._mappingInfos = null;
            this._resolveInfos?.Clear();
            this._resolveInfos = null;
            this._injectionInfos?.Clear();
            this._injectionInfos = null;
            this._globalShareInstanceCache?.Clear();
            this._globalShareInstanceCache = null;
            this._specifiedShareInstanceCache?.Clear();
            this._specifiedShareInstanceCache = null;
            this._localShareInstanceCache?.Clear();
            this._localShareInstanceCache = null;
        }

        private struct MappingInfo {
            private Type _implementorType;

            private HashSet<Type> _specifiedTypes; // 当前隐射关系只在这些类型里有效

            // Type: basicType, string: 名字(如果不是MemberInfo的话, 则名字为空)
            public Func<(Type, string), object> InstantiationFunc { get; set; }

            public Type ImplementorType => this._implementorType;

            public void SetImplementorType(Type implementorType) {
                if (implementorType == null)
                    throw new ArgumentNullException(nameof(implementorType));

                this._implementorType = implementorType;
            }

            public void AddSpecifiedType(Type type) {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                this._specifiedTypes ??= new();
                this._specifiedTypes.Add(type);
            }

            // 是否符合指定类型的条件
            public bool ConformSpecifiedType(Type type) {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                // 注意这里, 当_specifiedTypes为空时, 返回的是true, 意思是, 不限制某个类, 只要有需求, 都可以注入
                if (this._specifiedTypes == null)
                    return true;

                return this._specifiedTypes.Contains(type);
            }
        }

        private struct ResolveInfo {
            public bool AllowFieldInjection { get; set; }
        }

        private struct InjectionInfo {
            private ShareInjectionModel _shareInjectionModel;
            private HashSet<Type> _specifiedShareTypes;

            public ShareInjectionModel ShareInjectionModel => this._shareInjectionModel;

            public void SetShareInjectionModel(ShareInjectionModel shareInjectionModel) {
                this._shareInjectionModel = shareInjectionModel;
            }

            public void AddSpecifiedShareType(Type type) {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                this._specifiedShareTypes ??= new();
                this._specifiedShareTypes.Add(type);
            }

            // 是否符合指定类型的条件
            public bool ConformSpecifiedShareType(Type type) {
                if (type == null)
                    throw new ArgumentNullException(nameof(type));

                if (this._specifiedShareTypes == null)
                    throw new NullReferenceException(nameof(this._specifiedShareTypes));

                return this._specifiedShareTypes.Contains(type);
            }
        }
    }
}