using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    // 事件执行顺序
    //   添加一个组件时的执行顺序
    //     构造函数 -> initializeInvoke -> OnComponentAdd -> Awake -> Enable -> AheadUpdate -> Update -> LateUpdate -> Disable -> Destroy -> OnComponentRemove
    //   设置父级时的执行顺序
    //     BeforeParentChange -> OnChildRmove -> OnDomainChange -> OnParentChanged -> OnChildAdd
    [Serializable]
    [MemoryPackable()]
    public sealed partial class Entity : Object {
        private static int _cacherCount;

        public static int CacherCount => _cacherCount;

        // 在框架里, 只要是和组件类有关的 (包括父类、继承的接口), 都会被指定一个编号, 并记录再此, 该编号是唯一的, 该编号是按01234...顺序分配的,
        // 该编号被保存在ComponentTypeCacher的baseIndex中
        private static readonly Dictionary<int, ComponentTypeCacher> _typeHashtable = new(); // key: 类型的哈希值, value: 类型缓存器

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
            // 第一位是自己的编号, 后面的是他所有子类的编号, 但因为位列表是不存在严格意义上的"第一位"的, 所以我们需要一个额外的basIdx来记录每个类自己的基础编号

            // 第一步, 提前做一些缓存, 留作后用
            _typeHashtable.Clear();
            var map = new MultiList<Type, Type>(); // key: selfType, value: baseTypes
            var uniques = new HashSet<Type>(); // 所有会被分配编号的类、接口
            var componentType = typeof(Component);
            // 组件类也记录, 但不需要记录父类
            map.Add(componentType, null);
            uniques.Add(componentType);
            foreach (var type in EventSystem.GetAllTypes()) {
                if (type.IsSubclassOf(componentType)) {
                    if (!uniques.Contains(type)) uniques.Add(type);
                    // 如果该类是一个继承自组件的类, 则遍历其所有的父类
                    foreach (var baseType in AssemblyHelper.GetBaseTypes(type)) {
                        // 如果是一个接口类, 则直接添加
                        if (baseType.IsInterface) {
                            map.Add(type, baseType);
                            if (!uniques.Contains(baseType)) uniques.Add(baseType);
                        }
                        // 如果是class, 则要判断是不是组件类的子类, 因为我们最多缓存到组件类, 不再继续往上缓存
                        else {
                            if (baseType.IsSubclassOf(componentType)) {
                                map.Add(type, baseType);
                                if (!uniques.Contains(baseType)) uniques.Add(baseType);
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
                var hashCode = type.GetHashCode();
                var cacher = ComponentTypeCacher.Create(num++);
                cacher.Add(cacher.baseIndex);
                _typeHashtable[hashCode] = cacher;
            }

            // 第三步, 给每个类添加基类到映射表
            foreach (var kv in map) {
                var cacher = _typeHashtable[kv.Key.GetHashCode()];
                foreach (var baseType in kv.Value) {
                    if (baseType == null) continue;
                    _typeHashtable[baseType.GetHashCode()].Add(cacher.baseIndex);
                }
            }
        }

        public static ComponentTypeCacher CombineComponentType(IList<Type> types, int start = 0, int length = -1) {
            if (length == -1) length = types.Count;
            var result = ComponentTypeCacher.CreateNull();
            for (var i = start; i < length; i++) {
                result.Add(_typeHashtable[types[i].GetHashCode()].baseIndex);
            }

            return result;
        }

        public static ComponentTypeCacher CombineComponentType(params Type[] types) {
            var result = ComponentTypeCacher.CreateNull();
            for (int i = 0, len = types.Length; i < len; i++) {
                result.Add(_typeHashtable[types[i].GetHashCode()].baseIndex);
            }

            return result;
        }

        public static void CombineComponentType(ComponentTypeCacher cacher, Type type) {
            cacher.Add(_typeHashtable[type.GetHashCode()].baseIndex);
        }

        public static void CombineComponentType(ComponentTypeCacher cacher, params Type[] types) {
            for (int i = 0, len = types.Length; i < len; i++) {
                cacher.Add(_typeHashtable[types[i].GetHashCode()].baseIndex);
            }
        }

        public static int GetComponentIndex<T>() where T : Component {
            return GetComponentIndex(typeof(T));
        }

        public static int GetComponentIndex(Type componentType) {
            if (_typeHashtable.TryGetValue(componentType.GetHashCode(), out var cacher)) {
                return cacher.baseIndex;
            }

            return -1;
        }

        [MemoryPackInclude]
        internal string name;

        [MemoryPackIgnore]
        internal ComponentTypeCacher componentTypeCacher = ComponentTypeCacher.CreateNull(); // 这个位列表是作为组件索引的存在, 作用是极大的提高组件判存, 获取等操作的速度

        [MemoryPackIgnore]
        internal MultiList<int, Component> components; // key: 每个组件的编号, 这个编号是我们框架自行指定的1234...那个编号

        [MemoryPackInclude]
        internal List<Component> componentsOfSerialize;

        [MemoryPackIgnore]
        internal List<Entity> children;

        [MemoryPackInclude]
        internal List<Entity> childrenOfSerialize;

        [MemoryPackIgnore]
        internal Entity parent;

        [MemoryPackIgnore]
        internal Scene scene;

        [MemoryPackInclude]
        internal bool active = true;

        [BitListShowOfEnum("Hsenl.TagType")]
        [ShowInInspector, EnableGUI]
        [MemoryPackInclude]
        private Bitlist _tags = new();

        [MemoryPackIgnore]
        public bool Active {
            get => this.active;
            set {
                if (this.active == value) return;
                this.active = value;
                if (this.active) {
                    this.InternalOnActive();
                }
                else {
                    this.InternalOnInactive();
                }

                this.PartialOnActiveSelfChanged(value);
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
        public string Name => this.name;

        [MemoryPackIgnore]
        public Entity Parent => this.parent;

        [MemoryPackIgnore]
        public Bitlist Tags => this._tags;

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
            this.imminentDispose = false;
            EventSystemManager.Instance.RegisterInstanced(this);
        }

        // 实体的创建共两种, 一种是该方法, 另一种是反序列化, 最后还有一种特殊的, 就是unity序列化entity的时候, 会用到的默认构造函数
        private Entity(string name, Entity parent = null) {
            // 如果有父级, 则放到父级场景下, 没有则放到当前激活的场景下
            this.name = name;
            this.instanceId = Guid.NewGuid().GetHashCode();
            this.uniqueId = this.instanceId;
            this.imminentDispose = false;
            EventSystemManager.Instance.RegisterInstanced(this);
            this.PartialOnCreated();
        }

        public static Entity Create(string name, Entity parent = null) {
            var entity = new Entity(name, parent);
            entity.transform = entity.AddComponent<Transform>();
            entity.SetParent(parent);
            return entity;
        }

        protected override void BeginInit() {
            if (this.components != null) {
                this.componentsOfSerialize = new();
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        this.componentsOfSerialize.Add(component);
                    }
                }
            }

            if (this.children != null) {
                this.childrenOfSerialize = new();
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    this.childrenOfSerialize.Add(child);
                    child.BeginInit();
                }
            }
        }

        internal void Dispose() {
            this.name = null;
            this.componentTypeCacher?.Clear();
            this.componentTypeCacher = null;
            this.components?.Clear();
            this.componentsOfSerialize?.Clear();
            this.components = null;
            this.componentsOfSerialize = null;
            this.children?.Clear();
            this.childrenOfSerialize?.Clear();
            this.children = null;
            this.childrenOfSerialize = null;
            this.parent = null;
            this.scene = null;
            this._tags?.Clear();
            this._tags = null;
            this.uniqueId = 0;
            this.instanceId = 0;
            this.transform = null;
        }

        public void Reactivation() {
            this.Active = false;
            this.Active = true;
        }

        private MultiList<int, Component> GetOrCreateComponents() => this.components ??= new(); // basIdx, component

        private List<Entity> GetOrCreateChildren() => this.children ??= new();

        // 先把父子关系以及所有的组件先搭好, 待整个模型成型后, 再去触发应有的事件
        internal void InitializeBySerialization() {
            this.InitializeBySerizlizationRestorationRelation();
            this.InitializeBySerizlizationInvokeEvent();

            this.InternalOnSerialized();
        }

        private void InitializeBySerizlizationRestorationRelation() {
            // 把整个实体树的所有组件、子实体的关系都恢复好, 实例不用创建, 发序列化过后都实例化好的.
            if (this.componentsOfSerialize != null) {
                foreach (var componentSerialize in this.componentsOfSerialize) {
                    var type = componentSerialize.GetType();
                    if (!_typeHashtable.TryGetValue(type.GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
                    if (componentSerialize.entity != null)
                        throw new InvalidOperationException($"this component has been added to other entities '{this.Name}' '{componentSerialize.Name}'");

                    // 把组件的一些初始化的东西设置好
                    componentSerialize.entity = this;
                    componentSerialize.instanceId = Guid.NewGuid().GetHashCode();
                    componentSerialize.imminentDispose = false;

                    EventSystemManager.Instance.RegisterInstanced(componentSerialize);

                    this.componentTypeCacher.Add(cacher.baseIndex);
                    this.GetOrCreateComponents().Add(cacher.baseIndex, componentSerialize);

                    if (componentSerialize is Transform t) {
                        this.transform = t;
                    }
                }
            }

            if (this.childrenOfSerialize != null) {
                foreach (var childSerialize in this.childrenOfSerialize) {
                    this.GetOrCreateChildren().Add(childSerialize);
                    // 父子关系, 场景所属设置好
                    childSerialize.parent = this;
                    childSerialize.scene = this.scene;
                }

                foreach (var childSerialize in this.childrenOfSerialize) {
                    childSerialize.InitializeBySerizlizationRestorationRelation();
                }

                this.childrenOfSerialize = null;
            }
        }

        private void InitializeBySerizlizationInvokeEvent() {
            var realActive = this.RealActive;

            if (this.componentsOfSerialize != null) {
                for (int i = 0, len = this.componentsOfSerialize.Count; i < len; i++) {
                    var component = this.componentsOfSerialize[i];
                    if (component is IAheadUpdate aheadUpdate) EventSystem.RegisterAheadUpdate(aheadUpdate);
                    if (component is IUpdate update) EventSystem.RegisterUpdate(update);
                    if (component is ILateUpdate lateUpdate) EventSystem.RegisterLateUpdate(lateUpdate);

                    this.PartialOnComponentAdd(component);
                    component.InternalOnDeserialized();
                    // 这里向后顺位触发OnComponentAdd事件, 因为不能a触发b的同时, b也同时触发a, 这不符合逻辑
                    for (var j = i + 1; j < len; j++) {
                        this.componentsOfSerialize[j].InternalOnComponentAdd(component);
                    }

                    component.InternalOnConstruction();

                    if (realActive) {
                        component.InternalOnEnable();
                    }

                    if (!component.enable) {
                        component.enable = true;
                        component.InternalOnDisable();
                        component.enable = false;
                    }
                }

                this.componentsOfSerialize = null;
            }

            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    child.InternalOnParentChanged(null);
                    child.InternalOnDomainChanged();
                    this.InternalOnChildAdd(child);
                    child.PartialOnAfterParentChanged();
                    child.InternalOnAfterParentChanged(null);

                    child.InitializeBySerizlizationInvokeEvent();
                }
            }

            this.PartialOnActiveSelfChanged(false);
        }

        public void SetParent(Entity parentEntity, Scene targetScene = null) {
            this.SetParentInternal(parentEntity, targetScene);
        }

        internal void SetParentInternal(Entity parentEntity, Scene targetScene = null) {
            if (parentEntity == this)
                throw new Exception("you cannot set yourself as the parent");

            this.InternalOnBeforeParentChange(parentEntity); // 触发时, 一切都还没有变

            var oldScene = this.scene;
            var oldParent = this.parent;
            this.parent = parentEntity;

            this.InternalOnParentChanged(oldParent); // 父级改变的第一时间触发

            // 有父级, 则直接设置为父级的场景
            if (parentEntity != null) {
                this.SetScene(parentEntity.scene);
            }
            // 如果指定了场景, 则设置为指定场景
            else if (targetScene != null) {
                this.SetScene(targetScene);
            }
            // 既没有父级, 也没有指定场景, 则保持本来的场景, 如果缺少本来场景, 则设置为当前激活的场景
            else {
                if (this.scene == null) {
                    if (SceneManager.activeScene == null) {
                        throw new Exception("active scene is null");
                    }

                    this.SetScene(SceneManager.activeScene);
                }
            }

            if (oldScene != this.scene || oldParent != this.parent) {
                if (oldScene != null && oldParent == null) {
                    oldScene.rootEntities.Remove(this);
                }

                if (this.parent == null) {
                    this.scene.rootEntities.Add(this);
                }
            }

            oldParent?.InternalRemoveChild(this);
            this.parent?.InternalAddChild(this);
            this.PartialOnAfterParentChanged();
            this.InternalOnAfterParentChanged(oldParent); // 当触发时, 所有该变的都已经变过了
        }

        public int GetOrder() {
            if (this.parent == null) return -1;
            return this.parent.children.IndexOf(this);
        }

        public void SetOrder(int index) {
            if (this.parent == null) return;
            this.parent.children.Remove(this);
            this.parent.children.Insert(index, this);
        }

        public void DontDestroyOnLoad() {
            SceneManager.MoveEntityToScene(this, SceneManager.dontDestroyScene);
        }

        internal void SetScene(Scene value) {
            if (value == null)
                throw new NullReferenceException("set scene is null");

            if (this.scene != value) {
                this.RecursionSetScene(value);
            }
        }

        private void RecursionSetScene(Scene value) {
            this.scene = value;
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    this.children[i].RecursionSetScene(value);
                }
            }

            this.InternalOnDomainChanged();
        }

        private void InternalOnDomainChanged() {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnDomainChanged();
                    }
                }
            }
        }

        public T AddComponent<T>(bool enabled = true) where T : Component {
            return this.AddComponent<T>(null, enabled);
        }

        /// <summary>
        /// </summary>
        /// <param name="initializeInvoke">初始化委托, 该委托会在Awake之前被执行, 甚至在, 可以用来配置文件赋值</param>
        /// <param name="enabled"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T AddComponent<T>(Action<T> initializeInvoke, bool enabled = true) where T : Component {
            var type = typeof(T);
            if (!_typeHashtable.TryGetValue(type.GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            var component = (T)Activator.CreateInstance(type);
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.imminentDispose = false;
            component.enable = enabled;

            try {
                initializeInvoke?.Invoke(component);
            }
            catch (Exception e) {
                Log.Error($"<add component initialize delegate error> {e}");
            }

            EventSystemManager.Instance.RegisterInstanced(component);

            if (component is IAheadUpdate aheadUpdate) EventSystem.RegisterAheadUpdate(aheadUpdate);
            if (component is IUpdate update) EventSystem.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystem.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.baseIndex, component);

            component.InternalOnConstruction();

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        public Component AddComponent(Type type, Action<Component> initializeInvoke = null, bool enabled = true) {
            if (!_typeHashtable.TryGetValue(type.GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            var component = (Component)Activator.CreateInstance(type);
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.imminentDispose = false;
            component.enable = enabled;

            try {
                initializeInvoke?.Invoke(component);
            }
            catch (Exception e) {
                Log.Error($"<add component initialize delegate error> {e}");
            }

            EventSystemManager.Instance.RegisterInstanced(component);

            if (component is IAheadUpdate aheadUpdate) EventSystem.RegisterAheadUpdate(aheadUpdate);
            if (component is IUpdate update) EventSystem.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystem.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.baseIndex, component);

            component.InternalOnConstruction();

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        public Component AddComponent(Component component) {
            var type = component.GetType();
            if (!_typeHashtable.TryGetValue(type.GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{type.Name}'");
            component.entity = this;
            component.instanceId = Guid.NewGuid().GetHashCode();
            component.uniqueId = component.instanceId;
            component.imminentDispose = false;

            EventSystemManager.Instance.RegisterInstanced(component);

            if (component is IAheadUpdate aheadUpdate) EventSystem.RegisterAheadUpdate(aheadUpdate);
            if (component is IUpdate update) EventSystem.RegisterUpdate(update);
            if (component is ILateUpdate lateUpdate) EventSystem.RegisterLateUpdate(lateUpdate);

            this.InternalAddComponent(cacher.baseIndex, component);

            component.InternalOnConstruction();

            if (this.RealActive) {
                component.InternalOnEnable();
            }

            return component;
        }

        private void InternalAddComponent(int index, Component component) {
            this.componentTypeCacher.Add(index);
            this.GetOrCreateComponents().Add(index, component);
            // this.InternalAddComponentDB(component);
            this.PartialOnComponentAdd(component);
            this.InternalOnComponentAdd(component);
        }

        private void InternalAddComponentDB(Component component) {
            // if (!(component is ISerializable)) {
            //     return;
            // }

            this.componentsOfSerialize ??= new List<Component>();
            this.componentsOfSerialize.Add(component);
        }

        internal void InternalRemoveComponent(Component component) {
            if (this.components == null) return;
            if (component.entity != this) return;

            if (!_typeHashtable.TryGetValue(component.GetType().GetHashCode(), out var cacher))
                throw new Exception($"component type is not register '{component.GetType().Name}'");
            if (this.components.TryGetValue(cacher.baseIndex, out var list)) {
                list.Remove(component);
                if (list.Count == 0) {
                    this.componentTypeCacher.Remove(cacher.baseIndex);
                }
            }

            // this.InternalRemoveComponentDB(component);
            this.InternalOnComponentRemove(component);
        }

        private void InternalRemoveComponentDB(Component component) {
            if (this.componentsOfSerialize == null) return;
            // if (!(component is ISerializable)) {
            //     return;
            // }

            this.componentsOfSerialize.Remove(component);
        }

        public bool HasComponent<T>() where T : class {
            if (!_typeHashtable.TryGetValue(typeof(T).GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{typeof(T).Name}'");
            return this.componentTypeCacher.ContainsAny(cacher);
        }

        public bool HasComponentsAny(ComponentTypeCacher typeCacher) {
            return this.componentTypeCacher.ContainsAny(typeCacher);
        }

        public bool HasComponentsAny(ComponentTypeCacher typeCacher, out int idx) {
            return this.componentTypeCacher.ContainsAny(typeCacher, out idx);
        }

        public bool HasComponentsAll(ComponentTypeCacher typeCacher) {
            return this.componentTypeCacher.ContainsAll(typeCacher);
        }

        public T GetComponent<T>() where T : class {
            if (!_typeHashtable.TryGetValue(typeof(T).GetHashCode(), out var cacher))
                throw new Exception($"component type is not register '{typeof(T).Name}'");

            if (!this.componentTypeCacher.ContainsAny(cacher, out var idx)) return null;
            return this.components[idx][0] as T;
        }

        // 直接通过组件的编号来获取组件
        public Component GetComponent(int componentIndex) {
            if (this.components.TryGetValue(componentIndex, out var list))
                return list[0];

            return null;
        }

        // 直接通过组件的编号来获取组件
        public T GetComponent<T>(int componentIndex) where T : class {
            if (this.components.TryGetValue(componentIndex, out var list))
                return list[0] as T;

            return null;
        }

        public T[] GetComponents<T>() where T : class {
            using var list = ListComponent<T>.Create();
            this.InternalGetComponents(list);
            return list.ToArray();
        }

        public void GetComponents<T>(List<T> results) where T : class {
            this.InternalGetComponents(results);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponents<T>(List<T> results) where T : class {
            if (this.components == null) return;
            if (!_typeHashtable.TryGetValue(typeof(T).GetHashCode(), out var cacher)) throw new Exception($"component type is not register '{typeof(T).Name}'");
            foreach (var idx in this.componentTypeCacher.Contains(cacher)) {
                var list = this.components[idx];
                for (int i = 0, len = list.Count; i < len; i++) {
                    if (list[i] is T t) {
                        results.Add(t);
                    }
                }
            }
        }

        public Component[] GetComponentsOfTypeCacher(ComponentTypeCacher typeCacher) {
            using var list = ListComponent<Component>.Create();
            this.InternalGetComponentsOfTypeCacher(typeCacher, list);
            return list.ToArray();
        }

        public void GetComponentsOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results) {
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

        public T GetComponentInParent<T>(bool includeInactive = false) where T : class {
            var curr = this;
            while (curr != null) {
                if (!includeInactive && !curr.active) goto NEXT;
                var t = curr.GetComponent<T>();
                if (t != null) return t;

                NEXT:
                curr = curr.parent;
            }

            return null;
        }

        public T[] GetComponentsInParent<T>(bool includeInactive = false) where T : class {
            using var list = ListComponent<T>.Create();
            this.InternalGetComponentsInParent(list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInParent<T>(List<T> results, bool includeInactive = false) where T : class {
            this.InternalGetComponentsInParent(results, includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInParent<T>(List<T> results, bool includeInactive) where T : class {
            var curr = this;
            while (curr != null) {
                if (!includeInactive && !curr.active) goto NEXT;
                curr.InternalGetComponents(results);

                NEXT:
                curr = curr.parent;
            }
        }

        public Component[] GetComponentsInParentOfTypeCacher(ComponentTypeCacher typeCacher, bool includeInactive = false) {
            using var list = ListComponent<Component>.Create();
            this.InternalGetComponentsInParentOfTypeCacher(typeCacher, list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInParentOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive = false) {
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

        public T GetComponentInChildren<T>(bool includeInactive = false) where T : class {
            var t = this.GetComponent<T>();
            if (t != null) return t;
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!includeInactive && !child.active) continue;
                    t = child.GetComponentInChildren<T>(includeInactive);
                    if (t != null) return t;
                }
            }

            return null;
        }

        public T[] GetComponentsInChildren<T>(bool includeInactive = false) where T : class {
            using var list = ListComponent<T>.Create();
            this.InternalGetComponentsInChildren(list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInChildren<T>(List<T> result, bool includeInactive = false) where T : class {
            result.Clear();
            this.InternalGetComponentsInChildren(result, includeInactive);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InternalGetComponentsInChildren<T>(List<T> results, bool includeInactive) where T : class {
            this.InternalGetComponents(results);
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!includeInactive && !child.active) continue;
                    child.InternalGetComponentsInChildren(results, includeInactive);
                }
            }
        }

        public Component[] GetComponentsInChildrenOfTypeCacher(ComponentTypeCacher typeCacher, bool includeInactive = false) {
            using var list = ListComponent<Component>.Create();
            this.InternalGetComponentsInChildrenOfTypeCacher(typeCacher, list, includeInactive);
            return list.ToArray();
        }

        public void GetComponentsInChildrenOfTypeCacher(ComponentTypeCacher typeCacher, List<Component> results, bool includeInactive = false) {
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

        public void ForeachComponents(Action<Component> callback) {
            if (this.components == null) return;
            if (callback == null) throw new ArgumentNullException();
            foreach (var kv in this.components) {
                foreach (var component in kv.Value) {
                    callback.Invoke(component);
                }
            }
        }

        public void ForeachChildren(Action<Entity> callback, bool all = false) {
            if (this.children == null) return;
            if (callback == null) throw new ArgumentNullException();
            foreach (var child in this.children) {
                callback.Invoke(child);
                if (all) {
                    child.ForeachChildren(callback, true);
                }
            }
        }

        public ForeachChildrenEnumerable ForeachChildren() => new(this);

        public void Foreach(Action<Entity> callback) {
            if (callback == null) throw new ArgumentNullException();
            callback.Invoke(this);
            this.ForeachChildren(callback, true);
        }

        internal void ForeachSerialize(Action<Entity> callback) {
            if (callback == null) throw new ArgumentNullException();
            callback.Invoke(this);

            void ForeachChildrenSerialize(Entity entity, Action<Entity> action) {
                if (entity.childrenOfSerialize == null) return;
                foreach (var child in entity.childrenOfSerialize) {
                    action.Invoke(child);
                    ForeachChildrenSerialize(child, action);
                }
            }

            ForeachChildrenSerialize(this, callback);
        }

        private void InternalAddChild(Entity child) {
            this.GetOrCreateChildren().Add(child);
            // this.InternalAddChildDB(child);
            this.InternalOnChildAdd(child);
        }

        private void InternalAddChildDB(Entity child) {
            this.childrenOfSerialize ??= new List<Entity>();
            this.childrenOfSerialize.Add(child);
        }

        private void InternalRemoveChild(Entity child) {
            if (this.children == null) return;

            this.children.Remove(child);
            if (this.children.Count == 0)
                this.children = null;

            // this.InternalRemoveChildDB(child);
            this.InternalOnChildRemove(child);
        }

        private void InternalRemoveChildDB(Entity child) {
            this.childrenOfSerialize?.Remove(child);
        }

        public Entity GetChild(int index) {
            if (this.children == null) throw new ArgumentOutOfRangeException(index.ToString());
            return this.children[index];
        }

        public Entity FindChild(string childName) {
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (child.Name == childName) return child;
                }
            }

            return null;
        }

        public T FindChild<T>() where T : Component {
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

        public void SortChildren(Comparison<Entity> comparison) {
            this.children?.Sort(comparison);
        }

        public void SwapChild(int idx1, int idx2) {
            if (this.children == null) return;
            var child1 = this.children[idx1];
            var child2 = this.children[idx2];
            this.children[idx1] = child2;
            this.children[idx2] = child1;
        }

        // private void InternalOnSerialize() {
        //     if (this.components != null) {
        //         foreach (var kv in this.components) {
        //             foreach (var component in kv.Value) {
        //                 component.InternalOnDeserialize();
        //             }
        //         }
        //     }
        // }

        private void InternalOnSerialized() {
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
                    child.InternalOnSerialized();
                }
            }
        }

        // 通知所有组件, 自己的父级将要改变了
        private void InternalOnBeforeParentChange(Entity futrueParent) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnBeforeParentChange(futrueParent);
                    }
                }
            }
        }

        // 通知所有组件, 自己的父级已经改变了
        private void InternalOnParentChanged(Entity previousParent) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnParentChanged(previousParent);
                    }
                }
            }
        }

        private void InternalOnAfterParentChanged(Entity previousParent) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnAfterParentChanged(previousParent);
                    }
                }
            }
        }

        // 通知所有组件, 有新组件被添加
        private void InternalOnComponentAdd(Component component) {
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
        private void InternalOnComponentRemove(Component component) {
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
        private void InternalOnChildAdd(Entity child) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnChildAdd(child);
                    }
                }
            }
        }

        // 通知所有组件有子物体被移除
        private void InternalOnChildRemove(Entity child) {
            if (this.components != null) {
                foreach (var kv in this.components) {
                    foreach (var component in kv.Value) {
                        component.InternalOnChildRemove(child);
                    }
                }
            }
        }

        // 启用时, 自上而下, 自组件到子物体, 依次打开
        private void InternalOnActive() {
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
                    child.InternalOnActive();
                }
            }
        }

        // 禁用时, 自下而上, 自子物体到组件, 依次禁用
        private void InternalOnInactive() {
            if (this.children != null) {
                for (int i = 0, len = this.children.Count; i < len; i++) {
                    var child = this.children[i];
                    if (!child.active) continue;
                    child.InternalOnInactive();
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

        #region partial

        internal partial void PartialOnCreated();
        internal partial void PartialOnActiveSelfChanged(bool act);
        internal partial void PartialOnAfterParentChanged();
        internal partial void PartialOnComponentAdd(Component component);
        internal partial void PartialOnDestroy();

        #endregion

        public readonly struct ForeachChildrenEnumerable : IEnumerable<Entity> {
            private readonly Entity _entity;

            public ForeachChildrenEnumerable(Entity entity) {
                this._entity = entity;
            }

            public ForeachChildrenEnumerator GetEnumerator() => new(this._entity);
            IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator() => new ForeachChildrenEnumerator(this._entity);
            IEnumerator IEnumerable.GetEnumerator() => new ForeachChildrenEnumerator(this._entity);
        }

        public struct ForeachChildrenEnumerator : IEnumerator<Entity> {
            private Entity _entity;
            private int _index;

            public Entity Current { get; private set; }

            object IEnumerator.Current => this.Current;

            public ForeachChildrenEnumerator(Entity entity) {
                this._entity = entity;
                this.Current = null;
                this._index = 0;
            }

            public bool MoveNext() {
                if (this._entity.children == null) return false;
                if (this._index >= this._entity.children.Count) return false;
                this.Current = this._entity.children[this._index++];
                return true;
            }

            public void Reset() {
                this._entity = null;
                this._index = 0;
                this.Current = null;
            }

            public void Dispose() {
                this.Reset();
            }
        }
    }
}