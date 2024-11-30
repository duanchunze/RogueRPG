using System;
using MemoryPack;

#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [MemoryPackable(GenerateType.NoGenerate)]
    [ComponentOptions(ComponentMode = ComponentMode.Single)]
    public abstract partial class Component : Object, IComponent {
#if UNITY_EDITOR
        [ShowInInspector]
        [MemoryPackIgnore]
        public string ViewName => this.IsDisposed ? "Null" : this.Name;
#endif

        [NonSerialized] // unity那个 depth limit 10警告
        [MemoryPackIgnore]
        internal Entity entity;

        [MemoryPackIgnore]
        private bool _awaked;

        [MemoryPackOrder(1)]
        [MemoryPackInclude]
        internal bool enable = true;

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
                this.CheckDisposedException("Component is disposed, can't get entity");
                return this.entity;
            }
        }

        [MemoryPackIgnore]
        public Entity Parent => this.Entity.parent;

        [MemoryPackIgnore]
        public string Name => this.Entity.Name;

        [MemoryPackIgnore]
        public Scene Scene => this.Entity.scene;

#if UNITY_EDITOR
        [ShowInInspector]
#endif
        [MemoryPackIgnore]
        public bool Enable {
            get => this.enable;
            set {
                if (this.CheckDisposingLog())
                    return;

                this.CheckDisposedException("Component is disposed, can't set enable");
                if (this.enable == value)
                    return;

                this.enable = value;

                if (this.Entity.RealActive) {
                    if (this.enable) {
                        this.OnEnable();
                    }
                    else {
                        this.OnDisable();
                    }
                }

#if UNITY_EDITOR
                this.PartialOnEnableSelfChanged(value);
#endif
            }
        }

        [MemoryPackIgnore]
        public bool RealEnable {
            get {
                this.CheckDisposedException("Component is disposed");
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
            if (this.CheckDisposingLog())
                return;

            this.CheckDisposedException("Component is disposed, can't set enable");
            this.Enable = false;
            this.Enable = true;
        }

        internal sealed override void Dispose() {
            base.Dispose();
            this.entity = null;
            try {
                this.OnDisposedInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnDisposed();
            }
            catch (Exception e) {
                Log.Error(e);
            }
#if UNITY_EDITOR
            this.PartialOnDestroyFinish();
#endif
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

        internal void InternalOnAwake() {
            if (this._awaked)
                return;
            this._awaked = true;
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
            this.InternalOnAwake();

            try {
                this.OnEnable();
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        internal void InternalOnStart() {
            // if (this._start) return;
            // this._start = true;
            try {
                this.OnStartInternal();
            }
            catch (Exception e) {
                Log.Error(e);
            }

            try {
                this.OnStart();
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
            this._awaked = false;
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

        internal virtual void OnDeserializedInternal() { }

        // 当自己序列化完成时. 此时, 整个实体的组件, 父子, 都已经装配完毕, 只是此时还未触发任何事件.
        protected virtual void OnDeserialized() { }

        internal virtual void OnDeserializedOverallInternal() { }

        // 跟上面唯一的区别就是, 所有该触发的事件都触发了.
        protected virtual void OnDeserializedOverall() { }

        internal virtual void OnAwakeInternal() { }

        protected virtual void OnAwake() { }

        protected virtual void OnEnable() { }

        internal virtual void OnStartInternal() { }

        protected virtual void OnStart() { }

        protected virtual void OnDisable() { }

        internal virtual void OnDestroyInternal() { }

        protected virtual void OnDestroy() { }

        internal virtual void OnDisposedInternal() { }

        protected virtual void OnDisposed() { }

        internal virtual void OnBeforeParentChangeInternal(Entity futrueParent) { }

        protected virtual void OnBeforeParentChange(Entity futrueParent) { }

        internal virtual void OnParentChangedInternal(Entity previousParent) { }

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

#if UNITY_EDITOR
        partial void PartialOnEnableSelfChanged(bool enab);

        partial void PartialOnDestroyFinish();
#endif

        #endregion
    }
}