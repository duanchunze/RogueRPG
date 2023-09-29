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

        [NonSerialized]
        [MemoryPackIgnore]
        internal Entity entity;

        [MemoryPackIgnore]
        private bool _awake;

        [MemoryPackIgnore]
        protected bool IsAwake => this._awake;

        [MemoryPackOrder(10)]
        [MemoryPackInclude]
        internal bool enable = true;

        [MemoryPackIgnore]
        public int ComponentIndex {
            get {
                var index = Entity.GetComponentIndex(this.GetType());
                if (index == -1) throw new Exception($"get component index fail '{this.GetType().Name}'");
                return index;
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

        // internal Component() { }

        public void Reenable() {
            this.Enable = false;
            this.Enable = true;
        }

        internal void InternalOnDeserialized() {
            try {
                this.OnDeserialized();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnDeserializedOverall() {
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

        internal void InternalOnReset() {
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
        
        internal void InternalOnAfterParentChanged(Entity previousParent) {
            try {
                this.OnAfterParentChangedInternal(previousParent);
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnAfterParentChanged(previousParent);
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

        internal void InternalOnDomainChanged() {
            try {
                this.OnDomainChanged();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        // 当自己序列化完成时. 此时, 整个实体的组件, 父子, 都已经装配完毕, 只是此时还未触发任何事件.
        protected virtual void OnDeserialized() { }

        // 跟上面唯一的区别就是, 所有该触发的事件都触发了.
        protected virtual void OnDeserializedOverall() { }
        
        internal virtual void OnConstructionInternal() { }

        // 可以理解为当构造时, 但肯定没有构造函数早. 在Awake前一步执行, 且不受Enable == false影响. 因为添加组件的时候可以选择Enable为false, 所以增加了这个一个函数, 
        // 可以简单的把他当做一个不受Enable影响的Awake使用
        protected virtual void OnConstruction() { }

        internal virtual void OnAwakeInternal() { } // 专门用于内部方法

        // 常用于初始化系统方面的数据 (并不经常用到)
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

        // 之所以有了OnBeforeParentChange还要再写一个Internal版的, 是为了专门给内部一些系统使用的, 从而实际开发时, 重写方法时, 就不用纠结该方法是否需要实现base的方法.
        internal virtual void OnBeforeParentChangeInternal(Entity futrueParent) { }

        // 许多东西是很依赖父子关系的, 比如技能, 状态, 装备, 等等等等, 我们认为在拥有者物体下面即为被持有
        // 在这些东西父级发生变化时, 自然要更新自己的状态
        // 在unity中, 对象在被实例化出来的时候, 并不会触发父级改变事件. 即便 Instantiate 函数的父级参数填一个有效的, 也不会触发
        // 而且如果其本身就存在于某物体下面, 那么加载该父物体, 也不会触发该事件.
        // 父级改变前, 移除前父级的影响
        protected virtual void OnBeforeParentChange(Entity futrueParent) { }

        internal virtual void OnParentChangedInternal(Entity previousParent) { }

        // 父级在改变后, 添加新父级的影响
        protected virtual void OnParentChanged(Entity previousParent) { }
        
        internal virtual void OnAfterParentChangedInternal(Entity previousParent) { }

        // 父级在改变后, 添加新父级的影响
        protected virtual void OnAfterParentChanged(Entity previousParent) { }

        internal virtual void OnComponentAddInternal(Component component) { }

        protected virtual void OnComponentAdd(Component component) { }

        internal virtual void OnComponentRemoveInternal(Component component) { }

        protected virtual void OnComponentRemove(Component component) { }

        internal virtual void OnChildAddInternal(Entity child) { }

        protected virtual void OnChildAdd(Entity child) { }

        internal virtual void OnChildRemoveInternal(Entity child) { }

        protected virtual void OnChildRemove(Entity child) { }

        protected virtual void OnAppQuit() { }

        protected virtual void OnDomainChanged() { }

        #region partial

        partial void PartialOnEnableSelfChanged(bool enab);

        #endregion
    }
}