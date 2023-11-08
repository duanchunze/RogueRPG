using System;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Component : Object, IComponent {
        public static void CombineComponentType(Component lhs, Component rhs, ComponentTypeCacher typeCacher) {
            typeCacher.Add(lhs.Entity.componentTypeCacher);
            typeCacher.Add(rhs.Entity.componentTypeCacher);
        }

#if UNITY_EDITOR
        [ShowInInspector]
        [MemoryPackIgnore]
        public string ViewName => this.IsDisposed ? "Null" : this.Name;
#endif

        [NonSerialized]
        [MemoryPackIgnore]
        internal Entity entity;

        [MemoryPackIgnore]
        private bool _awake;

        [MemoryPackIgnore]
        protected bool IsAwake => this._awake;

        [MemoryPackOrder(1)]
        [MemoryPackInclude]
        protected internal bool enable = true;

        [MemoryPackIgnore]
        private int _componentIndex = -1;

        [MemoryPackIgnore]
        public int ComponentIndex {
            get {
                // component index 是由框架在这次进程, 给每个组件分配的唯一编号
                if (this._componentIndex == -1) {
                    this._componentIndex = Entity.GetComponentIndex(this.GetType());
                    if (this._componentIndex == -1)
                        throw new Exception($"get component index fail '{this.GetType().Name}'");
                }

                return this._componentIndex;
            }
        }

        [MemoryPackIgnore]
        public Entity Entity {
            get {
                if (this.IsDisposed) {
                    throw new NullReferenceException("The entity has been destroyed, but you're still trying to get it");
                }

                return this.entity;
            }
        }

        [MemoryPackIgnore]
        public Entity Parent => this.Entity.parent;

        [MemoryPackIgnore]
        public string Name => this.Entity.Name;

        [MemoryPackIgnore]
        public Scene Scene => this.Entity.scene;

        [MemoryPackIgnore]
        public bool Enable {
            get => this.enable;
            set {
                if (this.enable == value)
                    return;

                this.enable = value;

                if (this.Entity.RealActive) {
                    if (this.enable) {
                        this.InternalOnAwake();
                        this.OnEnable();
                    }
                    else {
                        this.OnDisable();
                    }
                }

                this.PartialOnEnableSelfChanged(value);
            }
        }

        [MemoryPackIgnore]
        public bool RealEnable {
            get {
                if (this.enable == false) return false;
                return this.Entity.RealActive;
            }
        }

        [MemoryPackIgnore]
        public int ChildCount => this.Entity.ChildCount;

        [MemoryPackIgnore]
        public Bitlist Tags => this.Entity.Tags;

        [MemoryPackIgnore]
        public Transform transform => this.Entity.transform;

        public void Reenable() {
            this.Enable = false;
            this.Enable = true;
        }

        internal void InternalOnDeserialized() {
            try {
                this.OnDeserializedInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }
            
            try {
                this.OnDeserialized();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnDeserializedOverall() {
            try {
                this.OnDeserializedOverallInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }
            
            try {
                this.OnDeserializedOverall();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnConstruction() {
            try {
                this.OnConstructionInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnConstruction();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        private void InternalOnAwake() {
            if (this._awake) return;
            this._awake = true;
            try {
                this.OnAwakeInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnAwake();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnEnable() {
            if (!this.enable) return;
            try {
                this.InternalOnAwake();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnEnable();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnDisable() {
            if (!this.enable) return;
            try {
                this.OnDisable();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnDestroy() {
            this._awake = false;
            try {
                this.OnDestroyInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnDestroy();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void Reset() {
            try {
                this.OnReset();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnBeforeParentChange(Entity futrueParent) {
            try {
                this.OnBeforeParentChangeInternal(futrueParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnBeforeParentChange(futrueParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnParentChanged(Entity previousParent) {
            try {
                this.OnParentChangedInternal(previousParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnParentChanged(previousParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnComponentAdd(Component component) {
            try {
                this.OnComponentAddInternal(component);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnComponentAdd(component);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnComponentRemove(Component component) {
            try {
                this.OnComponentRemoveInternal(component);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnComponentRemove(component);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnChildAdd(Entity child) {
            try {
                this.OnChildAddInternal(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnChildAdd(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnChildRemove(Entity child) {
            try {
                this.OnChildRemoveInternal(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnChildRemove(child);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnAppQuit() {
            try {
                this.OnAppQuit();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnSceneChanged() {
            try {
                this.OnSceneChanged();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        // 之所以每个函数都增加了一个Internal版本, 主要因为框架内部有时也需要用到这些函数, 但如此以来, 在框架外部实现该函数就必须要附带base.Function()的调用, 就会很乱
        // OnDeserialized
        // OnDeserializedOverall
        // OnConstruction
        // OnAwake
        // OnEnable
        // OnDisable
        // OnDestroy
        // OnBeforeParentChange
        // OnParentChanged
        // OnComponentAdd
        // OnComponentRemove
        // OnChildAdd
        // OnChildRemove
        // OnAppQuit
        // OnSceneChanged

        internal virtual void OnDeserializedInternal() { }
        
        // 当自己序列化完成时. 此时, 整个实体的组件, 父子, 都已经装配完毕, 只是此时还未触发任何事件.
        protected virtual void OnDeserialized() { }
        
        internal virtual void OnDeserializedOverallInternal() { }

        // 跟上面唯一的区别就是, 所有该触发的事件都触发了.
        protected virtual void OnDeserializedOverall() { }

        internal virtual void OnConstructionInternal() { }

        // 可以理解为当构造时, 但肯定没有构造函数早. 在Awake前一步执行, 且不受Enable == false影响. 因为添加组件的时候可以选择Enable为false, 所以增加了这个一个函数, 
        // 可以简单的把他当做一个不受Enable影响的Awake使用
        protected virtual void OnConstruction() { }

        internal virtual void OnAwakeInternal() { }

        // 常用于初始化系统方面的数据
        protected virtual void OnAwake() { }

        // 常用于初始化游戏逻辑方面的数据 (比如添加Control的监听事件)
        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        internal virtual void OnDestroyInternal() { }

        // dispose 的意义在于, 如果有其他地方做了自己的引用, 那么即使自己已经 destroy 了, 该引用依然能正常使用, 而我们可能并不知道已经被删除了, 所以, 我们要把数据全部清空, 达到一种提示的
        // 目的
        protected virtual void OnDestroy() { }

        // 对于一些需要还原的数据修改, 比如怪物的初始技能有三个, 然后打了一会架, 技能增加到了五个, 重置为初始状态后, 可用于对象池回收
        // 可以选择使用配置文件来初始化, 也可以手动的去初始化
        protected virtual void OnReset() { }

        // 父级改变时涉及到的函数
        // BeforeParent => (此时赋值改变了父级) => ParentChange => OnChildRemove => OnChildAdd => AfterParent

        // 实例化时不会触发
        internal virtual void OnBeforeParentChangeInternal(Entity futrueParent) { }

        protected virtual void OnBeforeParentChange(Entity futrueParent) { }

        internal virtual void OnParentChangedInternal(Entity previousParent) { }

        // 父级在改变后, 添加新父级的影响, 实例化时也会触发
        protected virtual void OnParentChanged(Entity previousParent) { }

        internal virtual void OnComponentAddInternal(Component component) { }

        protected virtual void OnComponentAdd(Component component) { }

        internal virtual void OnComponentRemoveInternal(Component component) { }

        protected virtual void OnComponentRemove(Component component) { }

        internal virtual void OnChildAddInternal(Entity child) { }

        protected virtual void OnChildAdd(Entity child) { }

        internal virtual void OnChildRemoveInternal(Entity child) { }

        protected virtual void OnChildRemove(Entity child) { }

        protected virtual void OnAppQuit() { }

        protected virtual void OnSceneChanged() { }

        #region partial

        partial void PartialOnEnableSelfChanged(bool enab);

        #endregion
    }
}