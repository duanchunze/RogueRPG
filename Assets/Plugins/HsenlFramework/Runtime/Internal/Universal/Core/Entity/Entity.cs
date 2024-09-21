using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    // 事件执行顺序
    //   添加一个组件时的执行顺序
    //     构造函数 -> initializeInvoke -> Awake -> OnComponentAdd -> Enable -> Start -> AheadUpdate -> Update -> LateUpdate -> Disable -> Destroy -> OnComponentRemove
    //   设置父级时的执行顺序
    //     BeforeParentChange -> OnParentChanged -> OnChildAdd -> OnChildRmove -> OnSceneChange
    //   序列化时的顺序
    //     OnDeserialized -> Awake -> Enable -> OnDeserializedOverall
    [Serializable]
    [MemoryPackable()]
    public sealed partial class Entity : Object {
        private static int _cacherCount;

        public static int CacherCount => _cacherCount;

        // 在框架里, 只要是和组件类有关的 (包括父类、继承的接口), 都会被指定一个编号, 并记录再此, 该编号是唯一的, 该编号是按01234...顺序分配的, 该编号为ComponentTypeIndex
        // ComponentTypeCacher.originalIndex == 该缓存器所代表的类型的编号
        // ComponentTypeCacher.bits 里保存了该类型所有的子类型的TypeIndex, 包括该类型自己的TypeIndex
        private static readonly Dictionary<Type, ComponentTypeCacher> _typeLookupTable = new(); // key: 某类型的哈希值, value: 某类型的'组件类型缓存器'

        private static readonly MultiQueue<Type, (ComponentTypeCacher require, Type add)> _requireComponentLookupTable = new();
        internal static readonly Dictionary<int, ComponentTypeCacher> requireInverseLookupTable = new();

        internal static readonly Dictionary<Type, ComponentOptionsAttribute> componentOptions = new();

        // 缓冲类型映射表是许多操作的基础, 所以应该被最先执行
        [OnEventSystemInitialized, Order(-500)]
        private static void CacheTypeHashtable() {
            // 获取所有组件类的类型的哈希映射表
            // 逻辑如此, 假设现在有3个类
            // Class1、2、3
            // 我们先给三个类规定一个编号, 就按照遍历的顺序来定, 那这三个类的编号分别是0 1 2
            // 三个类的继承关系是
            // Class1 是基类
            // Class2 继承自 Class1
            // Class3 继承自 Class2
            // 那么, 他们三个的哈希映射表分别是
            // Class1: 0, 1, 2
            // Class2: 1, 2
            // Class3: 2,
            // 第一位是自己的编号, 后面的是他所有子类的编号, 但因为位列表是不存在严格意义上的"第一位"的, 所以我们需要一个额外的originalIndex来记录每个类自己的原始编号

            // 第一步, 提前做一些缓存, 留作后用
            _typeLookupTable.Clear();
            var map = new MultiList<Type, Type>(); // key: selfType, value: baseTypes
            var uniques = new HashSet<Type>(); // 所有会被分配编号的类、接口
            var componentType = typeof(Component);
            // 组件类也记录, 但不需要记录父类
            map.Add(componentType, null);
            uniques.Add(componentType);
            foreach (var type in EventSystem.GetAllTypes()) {
                if (type.IsSubclassOf(componentType)) {
                    uniques.Add(type);
                    // 如果该类是一个继承自组件的类, 则遍历其所有的父类
                    foreach (var baseType in AssemblyHelper.GetBaseTypes(type)) {
                        // 如果是一个接口类, 则直接添加
                        if (baseType.IsInterface) {
                            map.Add(type, baseType);
                            uniques.Add(baseType);
                        }
                        // 如果是class, 则要判断是不是组件类的子类, 因为我们最多缓存到组件类, 不再继续往上缓存
                        else {
                            if (baseType.IsSubclassOf(componentType)) {
                                map.Add(type, baseType);
                                uniques.Add(baseType);
                            }
                        }
                    }

                    // 最后补充添加组件类
                    map.Add(type, componentType);
                }
            }

            var num = 0;
            _cacherCount = uniques.Count;
            // 第二步, 给每个类分配编号
            foreach (var type in uniques) {
                var cacher = ComponentTypeCacher.Create(num++);
                cacher.Add(cacher.originalIndex);
                _typeLookupTable[type] = cacher;
            }

            // 第三步, 给每个类添加基类到映射表
            foreach (var kv in map) {
                var cacher = _typeLookupTable[kv.Key];
                foreach (var baseType in kv.Value) {
                    if (baseType == null) continue;
                    _typeLookupTable[baseType].Add(cacher.originalIndex);
                }
            }
        }

        [OnEventSystemInitialized, Order(-400)]
        private static void CacheRequireTypes() {
            _requireComponentLookupTable.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(RequireComponentAttribute))) {
                var attr = type.GetCustomAttribute<RequireComponentAttribute>(true);
                if (_requireComponentLookupTable.TryGetValue(type, out var list1)) {
                    foreach (var tuple in list1) {
                        if (tuple.add == attr.addType) {
                            // 一个组件A重复Require了组件B
                            throw new Exception($"require component repetition, '{type.Name}' - '{attr.addType.Name}'");
                        }
                    }
                }

                if (_requireComponentLookupTable.TryGetValue(attr.addType, out var list2)) {
                    foreach (var tuple in list2) {
                        if (tuple.add == type) {
                            // 一个组件A Require了组件B, 同时, 组件B也Require了组件A
                            throw new Exception($"require component conflict, '{type.Name}' - '{attr.addType.Name}'");
                        }
                    }
                }

                var typeCacher = _typeLookupTable[attr.requireType];
                _requireComponentLookupTable.Enqueue(type, (typeCacher, attr.addType));
                var componentIndex = GetComponentIndex(type);
                if (!requireInverseLookupTable.TryGetValue(componentIndex, out var value)) {
                    value = ComponentTypeCacher.CreateNull();
                    requireInverseLookupTable[componentIndex] = value;
                }

                value.Add(GetComponentIndex(type));
            }
        }

        [OnEventSystemInitialized, Order(-400)]
        private static void CacheComponentOptions() {
            componentOptions.Clear();
            foreach (var type in EventSystem.GetTypesOfAttribute(typeof(ComponentOptionsAttribute))) {
                var attr = type.GetCustomAttribute<ComponentOptionsAttribute>(true);
                componentOptions[type] = attr;
            }
        }

        public static ComponentTypeCacher CombineComponentType(IList<Type> types, int start = 0, int length = -1) {
            if (length == -1) length = types.Count;
            var result = ComponentTypeCacher.CreateNull();
            for (var i = start; i < length; i++) {
                result.Add(_typeLookupTable[types[i]].originalIndex);
            }

            return result;
        }

        public static ComponentTypeCacher CombineComponentType(params Type[] types) {
            var result = ComponentTypeCacher.CreateNull();
            for (int i = 0, len = types.Length; i < len; i++) {
                result.Add(_typeLookupTable[types[i]].originalIndex);
            }

            return result;
        }

        public static void CombineComponentType(ComponentTypeCacher cacher, Type type) {
            cacher.Add(_typeLookupTable[type].originalIndex);
        }

        public static void CombineComponentType(ComponentTypeCacher cacher, params Type[] types) {
            for (int i = 0, len = types.Length; i < len; i++) {
                cacher.Add(_typeLookupTable[types[i]].originalIndex);
            }
        }

        public static int GetComponentIndex<T>() where T : class {
            return GetComponentIndex(typeof(T));
        }

        public static int GetComponentIndex(Type componentType) {
            if (_typeLookupTable.TryGetValue(componentType, out var cacher)) {
                return cacher.originalIndex;
            }

            return -1;
        }

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        [MemoryPackInclude]
        internal string name;

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackIgnore]
        internal ComponentTypeCacher componentTypeCacher = ComponentTypeCacher.CreateNull(); // 这个位列表是作为组件索引的存在, 作用是极大的提高组件判存, 获取等操作的速度

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackIgnore]
        internal MultiList<int, Component> components; // key: 每个组件的编号, 这个编号是我们框架自行指定的1234...那个编号

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackInclude]
        internal List<Component> componentsOfSerialize;

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackIgnore]
        internal List<Entity> children;

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackInclude]
        internal List<Entity> childrenOfSerialize;

        [MemoryPackIgnore]
        internal Entity parent;

        [MemoryPackIgnore]
        internal Scene scene;

#if UNITY_EDITOR
        [ShowInInspector, ReadOnly]
#endif
        [MemoryPackInclude]
        internal bool active = true;

        [BitListShowOfEnum("Hsenl.TagType")]
#if UNITY_EDITOR
        [ShowInInspector, EnableGUI]
#endif
        [MemoryPackInclude]
        internal Bitlist tags = new();

        [MemoryPackIgnore]
        public bool Active {
            get => this.active;
            set {
                this.CheckDisposingException("Entity is Disposed, can't set active");
                this.CheckDisposedException("Entity is Disposed, can't set active");
                if (this.active == value) return;
                this.active = value;
                if (this.active) {
                    this.OnActiveInternal();
                }
                else {
                    this.OnInactiveInternal();
                }

#if UNITY_5_3_OR_NEWER
                this.PartialOnActiveSelfChanged(value);
#endif
            }
        }

        // 实际激活状态, 综合自己和父级得出的结果, 只要有一环是未激活的, 那实际状态就是未激活
        [MemoryPackIgnore]
        public bool RealActive {
            get {
                var p = this;
                while (p != null) {
                    if (!p.active) return false;
                    p = p.parent;
                }

                return true;
            }
        }

        [MemoryPackIgnore]
        public string Name {
            get => this.name;
            set => this.name = value;
        }

        [MemoryPackIgnore]
        public Entity Parent => this.parent;

        [MemoryPackIgnore]
        public Scene Scene => this.scene;

        [MemoryPackIgnore]
        public Bitlist Tags => this.tags;

        [MemoryPackIgnore]
        public int ChildCount => this.children?.Count ?? 0;

        [MemoryPackIgnore]
        public Transform transform { get; internal set; }

        // public Entity() {
        //     // this.Init();
        //     // this.PartialOnCreated(); // 这里不能调用该方法, 牵扯到unity序列化可能会调用该构造函数, 导致一些意外操作, 比如创建、添加组件, 等操作
        // }

        // 这里预留一个无参构造函数, 一是为了不让外部创建, 二是留给unity序列化的时候用
        private Entity() { }

        // 这个构造函数是提供给MemoryPack用的
        [MemoryPackConstructor]
        private Entity(string name) {
            this.name = name;
            this.instanceId = Guid.NewGuid().GetHashCode();
            this.disposing = false;
            EventSystemManager.Instance.RegisterInstanced(this);
        }

        // 实体的创建共两种, 一种是该方法, 另一种是反序列化, 最后还有一种特殊的, 就是unity序列化entity的时候, 会用到的默认构造函数
        private Entity(string name, Entity parent = null) {
            // 如果有父级, 则放到父级场景下, 没有则放到当前激活的场景下
            if (name == "Manager") { }

            this.name = name;
            this.instanceId = Guid.NewGuid().GetHashCode();
            this.uniqueId = this.instanceId;
            this.disposing = false;
            EventSystemManager.Instance.RegisterInstanced(this);
#if UNITY_5_3_OR_NEWER
            this.PartialOnCreated();
#endif
        }

        public static Entity Create(string name, Entity parent = null) {
            var entity = new Entity(name, parent);
            entity.transform = entity.AddComponent<Transform>();
            entity.SetParent(parent);
            return entity;
        }

        protected override void BeginInit() {
            if (this.components != null) {
                this.componentsOfSerialize = new(this.components.Count);
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        this.componentsOfSerialize.Add(component);
                    }
                }
            }

            if (this.children != null) {
                this.childrenOfSerialize = new(this.children.Count);
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    this.childrenOfSerialize.Add(child);
                    child.BeginInit();
                }
            }
        }

        internal override void Dispose() {
            base.Dispose();
            this.name = null;
            this.componentTypeCacher?.Clear();
            this.componentTypeCacher = null;
            this.components?.Clear();
            this.components = null;
            this.componentsOfSerialize?.Clear();
            this.componentsOfSerialize = null;
            this.children?.Clear();
            this.children = null;
            this.childrenOfSerialize?.Clear();
            this.childrenOfSerialize = null;
            this.parent = null;
            this.scene = null;
            this.active = true;
            this.tags?.Clear();
            this.transform = null;
#if UNITY_5_3_OR_NEWER
            this.PartialOnDestroyFinish();
#endif
        }

        public void Reactivation() {
            this.CheckDisposingException("Entity is Disposed, can't set active");
            this.CheckDisposedException("Entity is Disposed, can't set active");
            this.Active = false;
            this.Active = true;
        }

        private MultiList<int, Component> GetOrCreateComponents(int capacity1 = 1, int capacity2 = 1)
            => this.components ??= new(capacity1, capacity2); // basIdx, components

        private List<Entity> GetOrCreateChildren(int capacity = 1) => this.children ??= new(capacity);

        // 先把父子关系以及所有的组件先搭好, 待整个模型成型后, 再去触发应有的事件
        internal void InitializeBySerialization() {
            this.InitializeBySerizlizationRestorationRelation();
            this.InitializeBySerizlizationInvokeEvent();

            this.OnSerializedOverallInternal();
        }

        private void InitializeBySerizlizationRestorationRelation() {
            // 把整个实体树的所有组件、子实体的关系都恢复好, 实例不用创建, 发序列化过后都实例化好的.
            if (this.componentsOfSerialize != null) {
                this.GetOrCreateComponents(this.componentsOfSerialize.Count);
                for (int i = 0, len = this.componentsOfSerialize.Count; i < len; i++) {
                    var component = componentsOfSerialize[i];
                    var type = component.GetType();
                    if (!_typeLookupTable.TryGetValue(type, out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
                    if (component.entity != null)
                        throw new InvalidOperationException($"this component has been added to other entities '{this.Name}' '{component.Name}'");

                    // 把组件的一些初始化的东西设置好
                    component.entity = this;
                    component.instanceId = Guid.NewGuid().GetHashCode();
                    component.disposing = false;

                    EventSystemManager.Instance.RegisterInstanced(component);

                    this.componentTypeCacher.Add(cacher.originalIndex);
                    this.GetOrCreateComponents().Add(cacher.originalIndex, component);

                    if (component is Transform t) {
                        this.transform = t;
                    }
                }
            }

            if (this.childrenOfSerialize != null) {
                this.GetOrCreateChildren(this.childrenOfSerialize.Count);
                for (int i = 0, len = this.childrenOfSerialize.Count; i < len; i++) {
                    var child = this.childrenOfSerialize[i];
                    this.GetOrCreateChildren().Add(child);
                    // 父子关系, 场景所属设置好
                    child.parent = this;
                    child.scene = this.scene;
                }

                for (int i = 0, len = this.childrenOfSerialize.Count; i < len; i++) {
                    var child = this.childrenOfSerialize[i];
                    child.InitializeBySerizlizationRestorationRelation();
                }

                this.childrenOfSerialize = null;
            }
        }

        private void InitializeBySerizlizationInvokeEvent() {
            var realActive = this.RealActive;

            if (this.componentsOfSerialize != null) {
                for (int i = 0, len = this.componentsOfSerialize.Count; i < len; i++) {
                    var component = this.componentsOfSerialize[i];
                    EventSystemManager.Instance.RegisterStart(component);
                    if (component is IUpdate update) EventSystemManager.Instance.RegisterUpdate(update);
                    if (component is ILateUpdate lateUpdate) EventSystemManager.Instance.RegisterLateUpdate(lateUpdate);

                    component.InternalOnDeserialized();

                    component.InternalOnAwake();

#if UNITY_5_3_OR_NEWER
                    this.PartialOnComponentAdd(component);
#endif
                    // 这里向后顺位触发OnComponentAdd事件, 因为不能a触发b的同时, b也同时触发a, 这不符合逻辑
                    // for (var j = i + 1; j < len; j++) {
                    //     this.componentsOfSerialize[j].InternalOnComponentAdd(component);
                    // }

                    if (realActive) {
                        component.InternalOnEnable();
                    }
                }

                this.componentsOfSerialize = null;
            }

            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
#if UNITY_5_3_OR_NEWER
                    child.PartialOnParentChanged();
#endif

                    child.InitializeBySerizlizationInvokeEvent();
                }
            }

#if UNITY_5_3_OR_NEWER
            this.PartialOnActiveSelfChanged(this.active);
#endif
        }

        public void SetParent(Entity value) {
            this.CheckDisposingException("Entity is Disposed, can't set parent");
            this.CheckDisposedException("Entity is Disposed, can't set parent");
            value?.CheckDisposingException("Entity is Disposed, can't be parent");
            value?.CheckDisposedException("Entity is Disposed, can't be parent");

            this.SetParentInternal(value, true);
        }

        internal void SetParentInternal(Entity futrueParent, bool setScene) {
            if (futrueParent == this)
                throw new Exception("you cannot set yourself as the parent");

            // 父级变化之前, 此时什么事情都还没有发生
            this.OnBeforeParentChangeInternal(futrueParent);

            // 先把父子关系处理好
            var prevParent = this.parent;
            this.parent = futrueParent;

            if (prevParent != null) {
                prevParent.children.Remove(this);
                if (prevParent.children.Count == 0) {
                    prevParent.children = null;
                }
            }

            this.parent?.GetOrCreateChildren().Add(this);

            // 然后触发事件
#if UNITY_5_3_OR_NEWER
            this.PartialOnParentChanged();
#endif
            this.OnParentChangedInternal(prevParent);
            prevParent?.OnChildRemoveInternal(this);
            this.parent?.OnChildAddInternal(this);
            
            if (!setScene)
                return;

            Scene futrueScene; // 设置父级是可能会改变实体的场景的
            // 有父级, 则直接设置为父级的场景
            if (futrueParent != null) {
                futrueScene = futrueParent.scene;
            }
            // 既没有父级, 自己也没有场景, 则设置为当前激活的场景
            else {
                futrueScene = this.scene;
                if (futrueScene == null) {
                    if (SceneManager.activeScene == null) {
                        throw new Exception("active scene is null");
                    }

                    futrueScene = SceneManager.activeScene;
                }
            }

            if (futrueScene == null) {
                Log.Error("BUG!"); // 设置父级时, 理应不该存在未来场景为空的情况.
            }

            // 说明存在切换场景的情况
            if (futrueScene != this.scene) {
                this.SetSceneInternal(futrueScene, prevParent, futrueParent);
            }
            // 没有切换场景, 则检查是否出现根实体变化的情况
            else {
                // 如果从无父级到有父级, 则从场景删除根实体
                if (prevParent == null && futrueParent != null) {
                    this.scene!.RemoveRootEntity(this);
                }

                // 如果从有父级到无父级, 则在场景添加根实体
                if (prevParent != null && futrueParent == null) {
                    this.scene!.AddRootEntity(this);
                }
            }
        }

        internal void SetSceneInternal(Scene value, Entity prevParent, Entity currParent) {
            if (this.scene == value)
                return;

            var prevScene = this.scene;
            this.scene = value;

            if (prevParent == null)
                prevScene?.RemoveRootEntity(this);
            if (currParent == null)
                value?.AddRootEntity(this);

            RecursionSetScene(this.children, value);
            this.OnSceneChangedInternal();

            return;

            void RecursionSetScene(List<Entity> childlist, Scene val) {
                if (childlist != null) {
                    for (int i = 0, len = childlist.Count; i < len; i++) {
                        var child = childlist[i];
                        childlist[i].scene = val;
                        RecursionSetScene(child.children, val);
                    }
                }
            }
        }

        public void DontDestroyOnLoad() {
            this.CheckDisposingException("Entity is disposed, can't move scene");
            this.CheckDisposedException("Entity is disposed, can't move scene");
            this.SetParent(null);
            SceneManager.MoveEntityToScene(this, SceneManager.dontDestroyScene);
        }

        public T AddComponent<T>(bool enabled = true) where T : Component {
            return this.AddComponent<T>(null, enabled);
        }

        /// <summary>
        /// </summary>
        /// <param name="initializeInvoke">初始化委托, 该委托会在Awake之前被执行, 可以用来配置文件赋值</param>
        /// <param name="enabled"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>(Action<T> initializeInvoke, bool enabled = true) where T : Component {
            if (this.CheckDisposingLog("Entity is disposed, can't add component"))
                return default;

            this.CheckDisposedException("Entity is disposed, can't add component");

            var type = typeof(T);
            if (componentOptions.TryGetValue(type, out var options)) {
                if (options.ComponentMode == ComponentMode.Single) {
                    if (this.HasComponent(type)) {
                        throw new InvalidOperationException($"Component mode is single, but you're still trying to add multiple '{type.FullName}'");
                    }
                }
            }

            if (_requireComponentLookupTable.TryGetValue(type, out var requires)) {
                foreach (var tuple in requires) {
                    if (!this.HasComponentsAny(tuple.require)) {
                        this.AddComponent(tuple.add);
                    }
                }
            }

            if (!_typeLookupTable.TryGetValue(type, out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            var component = (T)Activator.CreateInstance(type);
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.disposing = false;
            component.enable = enabled;

            try {
                initializeInvoke?.Invoke(component);
            }
            catch (Exception e) {
                Log.Error($"<add component initialize delegate error> {e}");
            }

            EventSystemManager.Instance.RegisterInstanced(component);

            EventSystemManager.Instance.RegisterStart(component);
            if (component is IUpdate update) EventSystemManager.Instance.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystemManager.Instance.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.originalIndex, component);

            component.InternalOnAwake();

#if UNITY_5_3_OR_NEWER
            this.PartialOnComponentAdd(component);
#endif
            this.OnComponentAddInternal(component);

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        public Component AddComponent(Type type, Action<Component> initializeInvoke = null, bool enabled = true) {
            if (this.CheckDisposingLog("Entity is disposed, can't add component"))
                return default;

            this.CheckDisposedException("Entity is disposed, can't add component");

            if (componentOptions.TryGetValue(type, out var options)) {
                if (options.ComponentMode == ComponentMode.Single) {
                    if (this.HasComponent(type)) {
                        throw new InvalidOperationException($"Component mode is single, but you're still trying to add multiple '{type.FullName}'");
                    }
                }
            }

            if (_requireComponentLookupTable.TryGetValue(type, out var requires)) {
                foreach (var tuple in requires) {
                    if (!this.HasComponentsAny(tuple.require)) {
                        this.AddComponent(tuple.add);
                    }
                }
            }

            if (!_typeLookupTable.TryGetValue(type, out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            var component = (Component)Activator.CreateInstance(type);
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.disposing = false;
            component.enable = enabled;

            try {
                initializeInvoke?.Invoke(component);
            }
            catch (Exception e) {
                Log.Error($"<add component initialize delegate error> {e}");
            }

            EventSystemManager.Instance.RegisterInstanced(component);

            EventSystemManager.Instance.RegisterStart(component);
            if (component is IUpdate update) EventSystemManager.Instance.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystemManager.Instance.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.originalIndex, component);

            component.InternalOnAwake();

#if UNITY_5_3_OR_NEWER
            this.PartialOnComponentAdd(component);
#endif
            this.OnComponentAddInternal(component);

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        public Component AddComponent(Component component) {
            if (this.CheckDisposingLog("Entity is disposed, can't add component"))
                return default;

            this.CheckDisposedException("Entity is disposed, can't add component");

            var type = component.GetType();
            if (componentOptions.TryGetValue(type, out var options)) {
                if (options.ComponentMode == ComponentMode.Single) {
                    if (this.HasComponent(type)) {
                        throw new InvalidOperationException($"Component mode is single, but you're still trying to add multiple '{type.FullName}'");
                    }
                }
            }

            if (_requireComponentLookupTable.TryGetValue(type, out var requires)) {
                foreach (var tuple in requires) {
                    if (!this.HasComponentsAny(tuple.require)) {
                        this.AddComponent(tuple.add);
                    }
                }
            }

            if (!_typeLookupTable.TryGetValue(type, out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.disposing = false;

            EventSystemManager.Instance.RegisterInstanced(component);

            EventSystemManager.Instance.RegisterStart(component);
            if (component is IUpdate update) EventSystemManager.Instance.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystemManager.Instance.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.originalIndex, component);

            component.InternalOnAwake();

#if UNITY_5_3_OR_NEWER
            this.PartialOnComponentAdd(component);
#endif
            this.OnComponentAddInternal(component);

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        private void InternalAddComponent(int index, Component component) {
            this.componentTypeCacher.Add(index);
            this.GetOrCreateComponents().Add(index, component);
        }

        internal void InternalRemoveComponent(Component component) {
            if (this.components == null) return;
            if (component.entity != this) return;

            if (!_typeLookupTable.TryGetValue(component.GetType(), out var cacher))
                throw new Exception($"component type is not register '{component.GetType().Name}'");
            if (this.components.TryGetValue(cacher.originalIndex, out var list)) {
                var remove = list.Remove(component);
                if (list.Count == 0) {
                    this.componentTypeCacher.Remove(cacher.originalIndex);
                }

                if (remove) {
                    this.OnComponentRemoveInternal(component);
                }
            }
        }

        public bool HasComponent<T>(bool polymorphic = false) where T : class {
            return this.HasComponent(typeof(T), polymorphic);
        }

        public bool HasComponent(Type type, bool polymorphic = false) {
            this.CheckDisposedException("Entity is disposed, can't check has component");
            if (this.components == null) return false;
            if (!_typeLookupTable.TryGetValue(type, out var cacher))
                throw new Exception($"component type is not register '{type.Name}'");

            if (!polymorphic) {
                return this.components.ContainsKey(cacher.originalIndex);
            }
            else {
                return this.componentTypeCacher.ContainsAny(cacher);
            }
        }

        public bool HasComponent(int componentIndex) {
            this.CheckDisposedException("Entity is disposed, can't check has component");
            return this.components.ContainsKey(componentIndex);
        }

        public bool HasComponentsAny(ComponentTypeCacher typeCacher) {
            this.CheckDisposedException("Entity is disposed, can't check has component");
            return this.componentTypeCacher.ContainsAny(typeCacher);
        }

        public bool HasComponentsAny(ComponentTypeCacher typeCacher, out int idx) {
            this.CheckDisposedException("Entity is disposed, can't check has component");
            return this.componentTypeCacher.ContainsAny(typeCacher, out idx);
        }

        public bool HasComponentsAll(ComponentTypeCacher typeCacher) {
            this.CheckDisposedException("Entity is disposed, can't check has component");
            return this.componentTypeCacher.ContainsAll(typeCacher);
        }

        // polymorphic为true代表获取自己和自己的多态
        public T GetComponent<T>(bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            if (!_typeLookupTable.TryGetValue(typeof(T), out var cacher))
                throw new Exception($"component type is not register '{typeof(T).Name}'");

            if (!polymorphic) {
                if (this.components.TryGetValue(cacher.originalIndex, out var list)) return list[0] as T;
                return null;
            }
            else {
                if (!this.componentTypeCacher.ContainsAny(cacher, out var idx)) return null;
                return this.components[idx][0] as T;
            }
        }

        // 直接通过组件的编号来获取组件
        public Component GetComponent(int componentIndex) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            if (this.components.TryGetValue(componentIndex, out var list))
                return list[0];

            return null;
        }

        // 直接通过组件的编号来获取组件
        public T GetComponent<T>(int componentIndex) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            if (this.components.TryGetValue(componentIndex, out var list))
                return list[0] as T;

            return null;
        }

        public T[] GetComponents<T>(bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<T>.Rent();
            this.InternalGetComponents(list, polymorphic);
            return list.ToArray();
        }

        public void GetComponents<T>(List<T> results, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            this.InternalGetComponents(results, polymorphic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponents<T>(List<T> results, bool polymorphic) where T : class {
            if (this.components == null) return;
            if (!_typeLookupTable.TryGetValue(typeof(T), out var cacher))
                throw new Exception($"component type is not register '{typeof(T).Name}'");

            if (!polymorphic) {
                if (this.components.TryGetValue(cacher.originalIndex, out var list)) {
                    for (int i = 0, len = list.Count; i < len; i++) {
                        if (list[i] is T t) {
                            results.Add(t);
                        }
                    }
                }
            }
            else {
                foreach (var idx in this.componentTypeCacher.Contains(cacher)) {
                    var list = this.components[idx];
                    for (int i = 0, len = list.Count; i < len; i++) {
                        if (list[i] is T t) {
                            results.Add(t);
                        }
                    }
                }
            }
        }

        public Component[] GetComponentsOfTypeCacher(ComponentTypeCacher typeCacher) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<Component>.Rent();
            this.InternalGetComponentsOfTypeCacher(typeCacher, list);
            return list.ToArray();
        }

        public void GetComponentsOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            this.InternalGetComponentsOfTypeCacher(typeCacher, results);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results) {
            if (this.components == null) return;
            foreach (var idx in this.componentTypeCacher.Contains(typeCacher)) {
                var list = this.components[idx];
                for (int i = 0, len = list.Count; i < len; i++) {
                    results.Add(list[i]);
                }
            }
        }

        public T GetComponentInParent<T>(bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            var curr = this;
            while (curr != null) {
                if (!includeInactive && !curr.active) goto NEXT;
                var t = curr.GetComponent<T>(polymorphic);
                if (t != null) return t;

                NEXT:
                curr = curr.parent;
            }

            return null;
        }

        public T[] GetComponentsInParent<T>(bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<T>.Rent();
            this.InternalGetComponentsInParent(list, includeInactive, polymorphic);
            return list.ToArray();
        }

        public void GetComponentsInParent<T>(List<T> results, bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            this.InternalGetComponentsInParent(results, includeInactive, polymorphic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInParent<T>(List<T> results, bool includeInactive, bool polymorphic) where T : class {
            var curr = this;
            while (curr != null) {
                if (!includeInactive && !curr.active) goto NEXT;
                curr.InternalGetComponents(results, polymorphic);

                NEXT:
                curr = curr.parent;
            }
        }

        public Component[] GetComponentsInParentOfTypeCacher(ComponentTypeCacher typeCacher, bool includeInactive = false) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<Component>.Rent();
            this.InternalGetComponentsInParentOfTypeCacher(typeCacher, list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInParentOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive = false) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            this.InternalGetComponentsInParentOfTypeCacher(typeCacher, results, includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInParentOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive) {
            var curr = this;
            while (curr != null) {
                if (!includeInactive && !curr.active) goto NEXT;
                curr.InternalGetComponentsOfTypeCacher(typeCacher, results);

                NEXT:
                curr = curr.parent;
            }
        }

        public T GetComponentInChildren<T>(bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            var t = this.GetComponent<T>(polymorphic);
            if (t != null) return t;
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!includeInactive && !child.active) continue;
                    t = child.GetComponentInChildren<T>(includeInactive, polymorphic);
                    if (t != null) return t;
                }
            }

            return null;
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<T>.Rent();
            this.InternalGetComponentsInChildren(list, includeInactive, polymorphic);
            return list.ToArray();
        }

        public void GetComponentsInChildren<T>(List<T> result, bool includeInactive = false, bool polymorphic = false) where T : class {
            this.CheckDisposedException("Entity is disposed, can't get component");
            result.Clear();
            this.InternalGetComponentsInChildren(result, includeInactive, polymorphic);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInChildren<T>(List<T> results, bool includeInactive, bool polymorphic) where T : class {
            this.InternalGetComponents(results, polymorphic);
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!includeInactive && !child.active) continue;
                    child.InternalGetComponentsInChildren(results, includeInactive, polymorphic);
                }
            }
        }

        public Component[] GetComponentsInChildrenOfTypeCacher(ComponentTypeCacher typeCacher, bool includeInactive = false) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            using var list = ListComponent<Component>.Rent();
            this.InternalGetComponentsInChildrenOfTypeCacher(typeCacher, list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInChildrenOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive = false) {
            this.CheckDisposedException("Entity is disposed, can't get component");
            this.InternalGetComponentsInChildrenOfTypeCacher(typeCacher, results, includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInChildrenOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive) {
            this.InternalGetComponentsOfTypeCacher(typeCacher, results);
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!includeInactive && !child.active) continue;
                    child.InternalGetComponentsInChildrenOfTypeCacher(typeCacher, results, includeInactive);
                }
            }
        }

        public void ForeachComponents<T>(Action<Component, T> callback, T data = default) {
            this.CheckDisposedException("Entity is disposed, can't foreach components");
            if (this.components == null) return;
            foreach (var kv in this.components) {
                var list = kv.Value;
                for (int i = 0, len = list.Count; i < len; i++) {
                    callback.Invoke(list[i], data);
                }
            }
        }

        public Iterator<Entity> ForeachChildren() {
            this.CheckDisposedException("Entity is disposed, can't foreach children");
            if (this.children == null)
                return default;

            return new Iterator<Entity>(this.children.GetEnumerator());
        }

        internal Iterator<Entity> ForeachSerializeChildren() {
            if (this.childrenOfSerialize == null)
                return default;

            return new Iterator<Entity>(this.childrenOfSerialize.GetEnumerator());
        }

        public bool IsParentOf(Entity targetParent) {
            if (this == targetParent)
                return true;

            var p = targetParent.parent;
            while (p != null) {
                if (p == this)
                    return true;

                p = p.parent;
            }

            return false;
        }

        public bool IsParentOf(Entity targetParent, out int layer) {
            layer = 0;
            var p = this.parent;
            while (p != null) {
                layer++;
                if (p == targetParent)
                    return true;
                p = p.parent;
            }

            layer = -1;
            return false;
        }

        public Entity GetChild(int index) {
            this.CheckDisposedException("Entity is disposed, can't get child");
            if (this.children == null) throw new ArgumentOutOfRangeException(index.ToString());
            return this.children[index];
        }

        public Entity FindChild(string childName) {
            this.CheckDisposedException("Entity is disposed, can't find child");
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (child.Name == childName) return child;
                }
            }

            return null;
        }

        public T FindChild<T>() where T : Component {
            this.CheckDisposedException("Entity is disposed, can't find child");
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    var t = child.GetComponent<T>();
                    if (t != null)
                        return t;
                }
            }

            return null;
        }

        public int GetOrder() {
            this.CheckDisposedException("Entity is disposed, can't get order");
            if (this.parent == null) return -1;
            return this.parent.children.IndexOf(this);
        }

        public void SetSiblingIndex(int index) {
            this.CheckDisposedException("Entity is disposed, can't set sibling index");
            if (this.parent == null) return;
            if (this.parent.children.Count <= 1) // 如果只有一个child, 则无需排序
                return;

            this.parent.children.Remove(this);
            this.parent.children.Insert(index, this);
#if UNITY_EDITOR
            this.PartialOnSetSiblingIndex(index);
#endif
        }

        public void SortChildren(Comparison<Entity> comparison) {
            this.CheckDisposedException("Entity is disposed, can't set sibling index");
            this.children?.Sort(comparison);
        }

        public void SwapChildren(int idx1, int idx2) {
            this.CheckDisposedException("Entity is disposed, can't swap children");
            if (this.children == null) return;

            if (idx1 < 0 || idx1 > this.children.Count)
                throw new ArgumentOutOfRangeException();

            if (idx2 < 0 || idx2 > this.children.Count)
                throw new ArgumentOutOfRangeException();

            var child1 = this.children[idx1];
            var child2 = this.children[idx2];
            this.children[idx1] = child2;
            this.children[idx2] = child1;
        }

        public void SwapChildren(Entity child1, Entity child2) {
            this.CheckDisposedException("Entity is disposed, can't swap children");

            if (child1 == null)
                throw new ArgumentNullException(nameof(child1));
            if (child2 == null)
                throw new ArgumentNullException(nameof(child2));

            this.children.SwapElements(child1, child2);
#if UNITY_EDITOR
            this.PartialOnSwapChildSeat(child1, child2);
#endif
        }

        private void OnSerializedOverallInternal() {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnDeserializedOverall();
                    }
                }
            }

            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    child.OnSerializedOverallInternal();
                }
            }
        }

        // 通知所有组件, 自己的父级将要改变了
        private void OnBeforeParentChangeInternal(Entity futrueParent) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnBeforeParentChange(futrueParent);
                    }
                }
            }
        }

        private void OnParentChangedInternal(Entity previousParent) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnParentChanged(previousParent);
                    }
                }
            }
        }

        private void OnSceneChangedInternal() {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnSceneChanged();
                    }
                }
            }

            Recursion(this.children);

            void Recursion(List<Entity> childlist) {
                if (childlist != null) {
                    for (int i = 0, len = childlist.Count; i < len; i++) {
                        var child = childlist[i];
                        if (child.components != null) {
                            foreach (var kv in child.components) {
                                foreach (var component in kv.Value) {
                                    component.InternalOnSceneChanged();
                                }
                            }
                        }
                    }

                    for (int i = 0, len = childlist.Count; i < len; i++) {
                        Recursion(childlist[i].children);
                    }
                }
            }
        }

        // 通知所有组件, 有新组件被添加
        private void OnComponentAddInternal(Component component) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var c in kv.Value) {
                        if (c == component) continue;
                        c.InternalOnComponentAdd(component);
                    }
                }
            }
        }

        // 通知所有组件, 有组件被移除
        private void OnComponentRemoveInternal(Component component) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var c in kv.Value) {
                        if (c == component) continue;
                        c.InternalOnComponentRemove(component);
                    }
                }
            }
        }

        // 通知所有组件, 有子物体被添加
        private void OnChildAddInternal(Entity child) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnChildAdd(child);
                    }
                }
            }
        }

        // 通知所有组件有子物体被移除
        private void OnChildRemoveInternal(Entity child) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnChildRemove(child);
                    }
                }
            }
        }

        // 启用时, 自上而下, 自组件到子物体, 依次打开
        private void OnActiveInternal() {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnEnable();
                    }
                }
            }

            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!child.active) continue;
                    child.OnActiveInternal();
                }
            }
        }

        // 禁用时, 自下而上, 自子物体到组件, 依次禁用
        private void OnInactiveInternal() {
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!child.active) continue;
                    child.OnInactiveInternal();
                }
            }

            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnDisable();
                    }
                }
            }
        }

        public override string ToString() {
            return $"{this.GetType()} '{this.name}'";
        }

        #region partial

#if UNITY_5_3_OR_NEWER
        internal partial void PartialOnCreated();
        internal partial void PartialOnActiveSelfChanged(bool act);
        internal partial void PartialOnParentChanged();
        internal partial void PartialOnComponentAdd(Component component);
        internal partial void PartialOnDestroyFinish();
        internal partial void PartialOnSetSiblingIndex(int index);
        internal partial void PartialOnSwapChildSeat(Entity child1, Entity child2);
#endif

        #endregion
    }
}