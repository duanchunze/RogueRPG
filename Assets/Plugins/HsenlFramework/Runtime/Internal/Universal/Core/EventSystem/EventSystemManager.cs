using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
using Sirenix.Utilities;
#endif

namespace Hsenl {
    [Serializable]
    public class EventSystemManager : Singleton<EventSystemManager> {
        internal Assembly[] Assemblies { get; private set; } = Array.Empty<Assembly>();

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly Dictionary<string, Type> _allTypes = new(); // key: type.FullName, value: type

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly MultiList<Type, Type> _typesOfAttribute = new(); // key: attribute type, values: types

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly MultiList<Type, EventInfo> _allEvents = new(); // key: event type

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        private readonly Dictionary<Type, InvokeInfo> _allInvokes = new(); // key: invoke type

        private readonly MultiList<Type, FieldInfo> _staticFieldsOfAttribute = new(); // key: FrameworkMemberAttribute type
        private readonly MultiList<Type, PropertyInfo> _staticPropertiesOfAttribute = new();
        private readonly MultiList<Type, MethodInfo> _staticMethodsOfAttribute = new();

        private readonly MultiDictionary<Type, Type, List<FieldInfo>> _instanceFieldsOfAttribute = new();
        private readonly MultiDictionary<Type, Type, List<PropertyInfo>> _instancePropertiesOfAttribute = new();
        private readonly MultiDictionary<Type, Type, List<MethodInfo>> _instanceMethodsOfAttribute = new();

        private readonly Queue<StartWrap> _starters = new();
        private readonly Queue<UpdateWrap> _updaters = new();
        private readonly Queue<LateUpdateWrap> _lateUpaters = new();

        private readonly Dictionary<int, Object> _instances = new();

        private readonly MultiDictionary<int, int, int> _links = new();

        private readonly HashSet<Type> _confineAttributeTypes = new() {
            typeof(BaseAttribute),
        };

        public void RegisterAttributeType(Type type) {
            this._confineAttributeTypes.Add(type);
        }

        public class AssemblesEqualityComparer : IEqualityComparer<Assembly> {
            public bool Equals(Assembly x, Assembly y) {
                // 自定义比较逻辑
                return x != null && x.Equals(y);
            }

            public int GetHashCode(Assembly obj) {
                // 返回自定义哈希码
                return obj.GetHashCode();
            }
        }

        public void SetAssembles(Assembly[] assemblies) {
            var mergedArray = this.Assemblies.Concat(assemblies).Distinct(new AssemblesEqualityComparer());
            this.Assemblies = mergedArray.ToArray();
            var types = AssemblyHelper.GetAssemblyTypes(assemblies);
            this.ClearCache();
            this.AddTypes(types);
            // 事件系统初始化完成
            foreach (var method in this.GetMethodsOfAttribute(typeof(OnEventSystemInitializedAttribute))) {
                if (!method.IsStatic) throw new InvalidOperationException($"EventSystemInitializedAttribute only use for static method '{method.Name}'");
                try {
                    method.Invoke(null, null);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        public void AddOrReplaceAssembles(Assembly[] assemblies) {
            List<Assembly> temp = new(this.Assemblies);
            foreach (var assembly in assemblies) {
                var replace = false;
                for (int i = 0, len = temp.Count; i < len; i++) {
                    if (temp[i].FullName == assembly.FullName) {
                        temp[i] = assembly;
                        replace = true;
                        break;
                    }
                }

                if (!replace) {
                    temp.Add(assembly);
                }
            }

            this.Assemblies = temp.ToArray();
            var types = AssemblyHelper.GetAssemblyTypes(assemblies);
            this.ClearCacheOfAssembles(assemblies);
            this.AddTypes(types);
            // 事件系统初始化完成
            foreach (var method in this.GetMethodsOfAttribute(typeof(OnEventSystemChangedAttribute))) {
                if (!method.IsStatic) throw new InvalidOperationException($"OnEventSystemChangedAttribute only use for static method '{method.Name}'");
                try {
                    method.Invoke(null, null);
                }
                catch (Exception e) {
                    Log.Error(e);
                }
            }
        }

        private void ClearCache() {
            this._allTypes.Clear();
            this._typesOfAttribute.Clear();
            this._allEvents.Clear();
            this._allInvokes.Clear();
            this._staticFieldsOfAttribute.Clear();
            this._staticPropertiesOfAttribute.Clear();
            this._staticMethodsOfAttribute.Clear();
            this._instanceFieldsOfAttribute.Clear();
            this._instancePropertiesOfAttribute.Clear();
            this._instanceMethodsOfAttribute.Clear();
        }

        private void ClearCacheOfAssembles(Assembly[] assemblies) {
            HashSet<string> fullNames = new();
            foreach (var assembly in assemblies) {
                fullNames.Add(assembly.FullName);
            }

            {
                var list = (from kv in this._allTypes where fullNames.Contains(kv.Value.Assembly.FullName) select kv.Key).ToList();
                foreach (var typeName in list) {
                    this._allTypes.Remove(typeName);
                }
            }

            {
                var list = new List<(Type attrType, Type type)>();
                foreach (var kv in this._typesOfAttribute) {
                    foreach (var type in kv.Value) {
                        if (!fullNames.Contains(type.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, type));
                    }
                }

                foreach (var tuple in list) {
                    this._typesOfAttribute.Remove(tuple.attrType, tuple.type);
                }
            }

            {
                var list = (from kv in this._allEvents where fullNames.Contains(kv.Key.Assembly.FullName) select kv.Key).ToList();
                foreach (var type in list) {
                    this._allEvents.Remove(type);
                }
            }

            {
                var list = (from kv in this._allInvokes where fullNames.Contains(kv.Key.Assembly.FullName) select kv.Key).ToList();
                foreach (var type in list) {
                    this._allInvokes.Remove(type);
                }
            }

            {
                var list = new List<(Type attrType, FieldInfo fieldInfo)>();
                foreach (var kv in this._staticFieldsOfAttribute) {
                    foreach (var fieldInfo in kv.Value) {
                        if (!fullNames.Contains(fieldInfo.DeclaringType?.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, fieldInfo));
                    }
                }

                foreach (var tuple in list) {
                    this._staticFieldsOfAttribute.Remove(tuple.attrType, tuple.fieldInfo);
                }
            }

            {
                var list = new List<(Type attrType, PropertyInfo propertyInfo)>();
                foreach (var kv in this._staticPropertiesOfAttribute) {
                    foreach (var propertyInfo in kv.Value) {
                        if (!fullNames.Contains(propertyInfo.DeclaringType?.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, propertyInfo));
                    }
                }

                foreach (var tuple in list) {
                    this._staticPropertiesOfAttribute.Remove(tuple.attrType, tuple.propertyInfo);
                }
            }

            {
                var list = new List<(Type attrType, MethodInfo methodInfo)>();
                foreach (var kv in this._staticMethodsOfAttribute) {
                    foreach (var methodInfo in kv.Value) {
                        if (!fullNames.Contains(methodInfo.DeclaringType?.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, methodInfo));
                    }
                }

                foreach (var tuple in list) {
                    this._staticMethodsOfAttribute.Remove(tuple.attrType, tuple.methodInfo);
                }
            }

            {
                var list = new List<(Type attrType, Type type)>();
                foreach (var kv in this._instanceFieldsOfAttribute) {
                    foreach (var kv2 in kv.Value) {
                        if (!fullNames.Contains(kv2.Key.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, kv2.Key));
                    }
                }

                foreach (var tuple in list) {
                    this._instanceFieldsOfAttribute.Remove(tuple.attrType, tuple.type);
                }
            }

            {
                var list = new List<(Type attrType, Type type)>();
                foreach (var kv in this._instancePropertiesOfAttribute) {
                    foreach (var kv2 in kv.Value) {
                        if (!fullNames.Contains(kv2.Key.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, kv2.Key));
                    }
                }

                foreach (var tuple in list) {
                    this._instancePropertiesOfAttribute.Remove(tuple.attrType, tuple.type);
                }
            }

            {
                var list = new List<(Type attrType, Type type)>();
                foreach (var kv in this._instanceMethodsOfAttribute) {
                    foreach (var kv2 in kv.Value) {
                        if (!fullNames.Contains(kv2.Key.Assembly.FullName))
                            continue;

                        list.Add((kv.Key, kv2.Key));
                    }
                }

                foreach (var tuple in list) {
                    this._instanceMethodsOfAttribute.Remove(tuple.attrType, tuple.type);
                }
            }
        }

        private void AddTypes(Type[] types) {
            foreach (var type in types) {
                if (type == null)
                    throw new NullReferenceException(nameof(type));

                this._allTypes[type.FullName!] = type;
            }

            // attribute ：types
            foreach (var type in types) {
                // 抽象类跳过, 但不包括静态类
                if (type.IsAbstract && !type.IsSealed) continue;

                var attrs = type.GetCustomAttributes(true);
                if (attrs.Length == 0) continue;

                foreach (var attr in attrs) {
                    var attrType = attr.GetType();
                    foreach (var confineAttributeType in this._confineAttributeTypes) {
                        if (attrType.IsSubclassOf(confineAttributeType)) {
                            this._typesOfAttribute.Add(attrType, type);
                            break;
                        }
                    }
                }
            }

            // events
            foreach (var type in this.GetTypesOfAttribute(typeof(EventAttribute))) {
                if (Activator.CreateInstance(type) is not IEvent obj) {
                    throw new Exception($"type not is AEvent: {type.Name}");
                }

                var order = 0;
                var orderAtt = CustomAttributeExtensions.GetCustomAttribute<OrderAttribute>(type, false);
                if (orderAtt != null) {
                    order = orderAtt.order;
                }

                this._allEvents.Add(obj.Type, new EventInfo() { iEvent = obj, eventModel = obj.EventModel, order = order });
            }

            // 把事件按优先级，从小到大排列
            foreach (var events in this._allEvents.Values) {
                events.Sort((o1, o2) => o1.order.CompareTo(o2.order));
            }

            // invokes
            foreach (var type in this.GetTypesOfAttribute(typeof(InvokeAttribute))) {
                if (Activator.CreateInstance(type) is not IInvoke obj) {
                    throw new Exception($"type not is AInvoke: {type.Name}");
                }

                this._allInvokes.Add(obj.Type, new InvokeInfo() { iInvoke = obj });
            }

            // attribute : classType ：members
            foreach (var typeRaw in types) {
                var fm = CustomAttributeExtensions.GetCustomAttribute<FrameworkMemberAttribute>(typeRaw, true);
                if (fm == null) continue;
                var type = typeRaw;
                if (type.IsGenericType) continue;

                // 成员获取规则:
                // 非静态成员, 获取自己和父级的所有的成员, 因为非静态成员有类的类型作key, 不会重复
                // 而静态成员, 只获取自己的所有成员
                var members = ArrayHelper.Combin(type.GetMembersInBase(AssemblyHelper.BindingFlagsInstanceIgnorePublic), type.GetMembers(AssemblyHelper.All));
                foreach (var member in members) {
                    var objects = member.GetCustomAttributes(false);
                    if (objects.Length == 0) continue;

                    foreach (var o in objects) {
                        var attType = o.GetType();
                        var b = false;
                        foreach (var confineAttributeType in this._confineAttributeTypes) {
                            if (attType.IsSubclassOf(confineAttributeType)) {
                                b = true;
                                break;
                            }
                        }

                        if (b) {
                            switch (member) {
                                case FieldInfo fieldInfo: {
                                    if (fieldInfo.IsStatic) {
                                        this._staticFieldsOfAttribute.Add(attType, fieldInfo);
                                    }
                                    else {
                                        if (!this._instanceFieldsOfAttribute.TryGetValue(attType, type, out var list)) {
                                            this._instanceFieldsOfAttribute[attType, type] = new List<FieldInfo>() { fieldInfo };
                                            break;
                                        }

                                        list.Add(fieldInfo);
                                    }

                                    break;
                                }

                                case PropertyInfo propertyInfo: {
                                    if (!propertyInfo.CanRead ? propertyInfo.GetSetMethod(true).IsStatic : propertyInfo.GetGetMethod(true).IsStatic) {
                                        this._staticPropertiesOfAttribute.Add(attType, propertyInfo);
                                    }
                                    else {
                                        if (!this._instancePropertiesOfAttribute.TryGetValue(attType, type, out var list)) {
                                            this._instancePropertiesOfAttribute[attType, type] = new List<PropertyInfo>() { propertyInfo };
                                            break;
                                        }

                                        list.Add(propertyInfo);
                                    }

                                    break;
                                }

                                case MethodInfo methodInfo: {
                                    if (methodInfo.IsStatic) {
                                        this._staticMethodsOfAttribute.Add(attType, methodInfo);
                                    }
                                    else {
                                        if (!this._instanceMethodsOfAttribute.TryGetValue(attType, type, out var list)) {
                                            this._instanceMethodsOfAttribute[attType, type] = new List<MethodInfo>() { methodInfo };
                                            break;
                                        }

                                        list.Add(methodInfo);
                                    }

                                    break;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var kv in this._staticFieldsOfAttribute) {
                kv.Value.Sort((o1, o2) => {
                    var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    return order1.CompareTo(order2);
                });
            }

            foreach (var kv in this._staticPropertiesOfAttribute) {
                kv.Value.Sort((o1, o2) => {
                    var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    return order1.CompareTo(order2);
                });
            }

            foreach (var kv in this._staticMethodsOfAttribute) {
                kv.Value.Sort((o1, o2) => {
                    var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                    return order1.CompareTo(order2);
                });
            }

            foreach (var kv in this._instanceFieldsOfAttribute) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Sort((o1, o2) => {
                        var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        return order1.CompareTo(order2);
                    });
                }
            }

            foreach (var kv in this._instancePropertiesOfAttribute) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Sort((o1, o2) => {
                        var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        return order1.CompareTo(order2);
                    });
                }
            }

            foreach (var kv in this._instanceMethodsOfAttribute) {
                foreach (var kv2 in kv.Value) {
                    kv2.Value.Sort((o1, o2) => {
                        var order1 = o1.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        var order2 = o2.GetCustomAttribute<OrderAttribute>()?.order ?? 0;
                        return order1.CompareTo(order2);
                    });
                }
            }
        }

        public Assembly[] GetAssemblies() {
            var copy = new Assembly[this.Assemblies.Length];
            Array.Copy(this.Assemblies, copy, copy.Length);
            return copy;
        }

        public Type[] GetAllTypes() {
            return this._allTypes.Values.ToArray();
        }

        public Type FindType(string typeName) {
            this._allTypes.TryGetValue(typeName, out var result);
            return result;
        }

        /// <param name="attributeType"></param>
        /// <param name="polymorphism">是否支持多态</param>
        /// <returns></returns>
        public IReadOnlyList<Type> GetTypesOfAttribute(Type attributeType, bool polymorphism = false) {
            if (!polymorphism) {
                return this._typesOfAttribute.TryGetValue(attributeType, out var result) ? result : Type.EmptyTypes;
            }
            else {
                List<Type> list = new();
                foreach (var kv in this._typesOfAttribute) {
                    if (kv.Key.IsSubclassOf(attributeType)) {
                        list.AddRange(kv.Value);
                    }
                }

                return list;
            }
        }

        public IReadOnlyList<FieldInfo> GetFieldsOfAttribute(Type attributeType, Type classType = null) {
            if (classType == null) {
                return this._staticFieldsOfAttribute.TryGetValue(attributeType, out var result) ? result : Array.Empty<FieldInfo>();
            }
            else {
                return this._instanceFieldsOfAttribute.TryGetValue(attributeType, classType, out var result) ? result : Array.Empty<FieldInfo>();
            }
        }

        public IReadOnlyList<PropertyInfo> GetPropertiesOfAttribute(Type attributeType, Type classType = null) {
            if (classType == null) {
                return this._staticPropertiesOfAttribute.TryGetValue(attributeType, out var result) ? result : Array.Empty<PropertyInfo>();
            }
            else {
                return this._instancePropertiesOfAttribute.TryGetValue(attributeType, classType, out var result) ? result : Array.Empty<PropertyInfo>();
            }
        }

        public IReadOnlyList<MethodInfo> GetMethodsOfAttribute(Type attributeType, Type classType = null) {
            if (classType == null) {
                return this._staticMethodsOfAttribute.TryGetValue(attributeType, out var result) ? result : Array.Empty<MethodInfo>();
            }
            else {
                return this._instanceMethodsOfAttribute.TryGetValue(attributeType, classType, out var result) ? result : Array.Empty<MethodInfo>();
            }
        }

        public void SetEventEnable<TEventKey, TEvent>(bool value) {
            if (!this._allEvents.TryGetValue(typeof(TEventKey), out var eventList)) {
                return;
            }

            for (int i = 0, len = eventList.Count; i < len; i++) {
                var eventInfo = eventList[i];
                if (eventInfo is not TEvent) continue;
                eventInfo.enable = value;
                break;
            }
        }

        internal void RegisterInstanced(Object instance) {
            this._instances.Add(instance.InstanceId, instance);
        }

        internal void UnregisterInstanced(int instanceId) {
            this._instances.Remove(instanceId);
        }

        public Object GetInstance(int instanceId) {
            this._instances.TryGetValue(instanceId, out var instance);
            return instance;
        }

        public T GetInstance<T>(int instanceId) where T : Object {
            this._instances.TryGetValue(instanceId, out var instance);
            return instance as T;
        }

        public Object[] GetAllInstance() {
            return this._instances.Values.ToArray();
        }

        internal Dictionary<int, Object>.ValueCollection GetAllInstanceRef() {
            return this._instances.Values;
        }

        internal void RegisterStart(Component starter) {
            this._starters.Enqueue(new StartWrap() { instanceId = starter.InstanceId, starter = starter });
        }

        internal void RegisterUpdate(IUpdate update) {
            this._updaters.Enqueue(new UpdateWrap() { instanceId = update.InstanceId, updater = update });
        }

        internal void RegisterLateUpdate(ILateUpdate update) {
            this._lateUpaters.Enqueue(new LateUpdateWrap() { instanceId = update.InstanceId, updater = update });
        }

        public async HTask PublishAsync<T>(T a) where T : struct {
            if (!this._allEvents.TryGetValue(typeof(T), out var eventList)) {
                return;
            }

            // == 0的事件, 是并行执行的, 大于或小于0的事件, 是按顺序依次执行的, 这么做的目的是为了保持并行的速度, 因为有时候我们可能并不在意执行顺序, 所以如果都统一按顺序依次执行,
            // 对于异步事件来说, 会大大影响速度
            using var asyncEvents = ListComponent<HTask>.Create();

            for (int i = 0, len = eventList.Count; i < len; i++) {
                var eventInfo = eventList[i];
                if (!eventInfo.enable) continue;
                switch (eventInfo.eventModel) {
                    case EventModel.Sync:
                        var syncEvent = (AEventSync<T>)eventInfo.iEvent;
                        switch (eventInfo.order) {
                            case < 0:
                                try {
                                    syncEvent.Invoke(a);
                                }
                                catch (Exception e) {
                                    Log.Error(e);
                                }

                                break;
                            case 0:
                                try {
                                    syncEvent.Invoke(a);
                                }
                                catch (Exception e) {
                                    Log.Error(e);
                                }

                                break;
                            default: {
                                if (asyncEvents.Count != 0) {
                                    try {
                                        await HTask.WaitAll(asyncEvents);
                                    }
                                    catch (Exception e) {
                                        Log.Error(e);
                                    }

                                    asyncEvents.Clear();
                                }

                                try {
                                    syncEvent.Invoke(a);
                                }
                                catch (Exception e) {
                                    Log.Error(e);
                                }

                                break;
                            }
                        }

                        break;
                    case EventModel.Async:
                        var asyncEvent = (AEventAsync<T>)eventInfo.iEvent;
                        switch (eventInfo.order) {
                            case < 0:
                                try {
                                    await asyncEvent.Invoke(a);
                                }
                                catch (Exception e) {
                                    Log.Error(e);
                                }

                                break;
                            case 0:
                                asyncEvents.Add(asyncEvent.Invoke(a));
                                break;
                            default: {
                                if (asyncEvents.Count != 0) {
                                    try {
                                        await HTask.WaitAll(asyncEvents);
                                    }
                                    catch (Exception e) {
                                        Log.Error(e);
                                    }

                                    asyncEvents.Clear();
                                }

                                try {
                                    await asyncEvent.Invoke(a);
                                }
                                catch (Exception e) {
                                    Log.Error(e);
                                }

                                break;
                            }
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            try {
                if (asyncEvents.Count != 0) {
                    await HTask.WaitAll(asyncEvents);
                }
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void Publish<T>(T a) where T : struct {
            if (!this._allEvents.TryGetValue(typeof(T), out var eventList)) {
                return;
            }

            for (int i = 0, len = eventList.Count; i < len; i++) {
                var eventInfo = eventList[i];
                if (!eventInfo.enable) continue;
                switch (eventInfo.eventModel) {
                    case EventModel.Sync:
                        var syncEvent = (AEventSync<T>)eventInfo.iEvent;
                        try {
                            syncEvent.Invoke(a);
                        }
                        catch (Exception e) {
                            Log.Error(e);
                        }

                        break;
                    case EventModel.Async:
                        var asyncEvent = (AEventAsync<T>)eventInfo.iEvent;
                        try {
                            asyncEvent.Invoke(a).Tail();
                        }
                        catch (Exception e) {
                            Log.Error(e);
                        }

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public void Invoke<TArg>(TArg args) where TArg : struct {
            if (!this._allInvokes.TryGetValue(typeof(TArg), out var invokeInfo)) {
                throw new Exception($"Invoke error: {typeof(TArg).Name}");
            }

            var aInvokeHandler = (AInvokeHandler<TArg>)invokeInfo.iInvoke;
            aInvokeHandler.Handle(args);
        }

        public TReturn Invoke<TArg, TReturn>(TArg args) where TArg : struct {
            if (!this._allInvokes.TryGetValue(typeof(TArg), out var invokeInfo)) {
                throw new Exception($"Invoke error: {typeof(TArg).Name}");
            }

            var aInvokeHandler = (AInvokeHandler<TArg, TReturn>)invokeInfo.iInvoke;
            return aInvokeHandler.Handle(args);
        }

        public void Update() {
            var startersCount = this._starters.Count;
            while (startersCount-- > 0) {
                var starter = this._starters.Dequeue();
                if (starter.starter.InstanceId != starter.instanceId)
                    continue;

                if (starter.starter.RealEnable) {
                    starter.starter.InternalOnStart();
                    continue;
                }

                this._starters.Enqueue(starter);
            }

            var updatersCount = this._updaters.Count;
            while (updatersCount-- > 0) {
                var updater = this._updaters.Dequeue();
                if (updater.updater.InstanceId != updater.instanceId)
                    continue;

                if (updater.updater.RealEnable) {
                    try {
                        updater.updater.Update();
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }
                }

                this._updaters.Enqueue(updater);
            }
        }

        public void LateUpdate() {
            var count = this._lateUpaters.Count;
            while (count-- > 0) {
                var updater = this._lateUpaters.Dequeue();
                if (updater.updater.InstanceId != updater.instanceId)
                    continue;

                if (updater.updater.RealEnable) {
                    try {
                        updater.updater.LateUpdate();
                    }
                    catch (Exception e) {
                        Log.Error(e);
                    }
                }

                this._lateUpaters.Enqueue(updater);
            }
        }

        public void Link<T>(int instanceId, int instanceIdTarget, int key = 0) where T : Object {
            if (key != 0) key = HashCode.Combine(typeof(T).GetHashCode(), key);
            this.Link(instanceId, instanceIdTarget, key);
        }

        public void Link(int instanceId, int instanceIdTarget, int key) {
            if (instanceId == 0 || instanceIdTarget == 0) throw new Exception("link fail instance id cant be 0");
            this._links.Add(instanceId, key, instanceIdTarget);
        }

        // 断开所有链接
        public void Unlink(int instanceId) {
            if (instanceId == 0) throw new Exception("unlink fail instance id cant be 0");
            this._links.Remove(instanceId);
        }

        public void Unlink<T>(int instanceId, int key = 0) where T : Object {
            if (key != 0) key = HashCode.Combine(typeof(T).GetHashCode(), key);
            this._links.Remove(instanceId, key);
        }

        public void Unlink(int instanceId, int key) {
            if (instanceId == 0) throw new Exception("unlink fail instance id cant be 0");
            this._links.Remove(instanceId, key);
        }

        public T GetLinker<T>(int instanceId, int key = 0) where T : Object {
            if (key != 0) key = HashCode.Combine(typeof(T).GetHashCode(), key);
            if (instanceId == 0) throw new Exception("get linker fail instance id cant be 0");
            this._links.TryGetValue(instanceId, key, out var linerInstanceId);
            this._instances.TryGetValue(linerInstanceId, out var liner);
            return liner as T;
        }

        protected override void OnUnregister() {
            this.Assemblies = Array.Empty<Assembly>();
            this._allTypes.Clear();
            this._typesOfAttribute.Clear();
            this._allEvents.Clear();
            this._allInvokes.Clear();
            this._staticFieldsOfAttribute.Clear();
            this._staticPropertiesOfAttribute.Clear();
            this._staticMethodsOfAttribute.Clear();
            this._instanceFieldsOfAttribute.Clear();
            this._instancePropertiesOfAttribute.Clear();
            this._instanceMethodsOfAttribute.Clear();
            this._starters.Clear();
            this._updaters.Clear();
            this._lateUpaters.Clear();
            this._instances.Clear();
            this._links.Clear();
            this._confineAttributeTypes.Clear();
            this._confineAttributeTypes.Add(typeof(BaseAttribute));
        }

        private class EventInfo {
            public IEvent iEvent;
            public EventModel eventModel;
            public int order;
            public bool enable = true;
        }

        private class InvokeInfo {
            public IInvoke iInvoke;
        }

        private struct StartWrap {
            public int instanceId;
            public Component starter;
        }

        private struct UpdateWrap {
            public int instanceId;
            public IUpdate updater;
        }

        private struct LateUpdateWrap {
            public int instanceId;
            public ILateUpdate updater;
        }
    }
}