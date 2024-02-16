using System;
using System.Collections.Generic;
using MemoryPack;
using Sirenix.OdinInspector;

namespace Hsenl {
    [FrameworkMember]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Object {
        private static readonly List<Component> _destroyComponentsCache = new();
        private static readonly List<Component> _destroyComponentsCacheOnce = new();
        private static readonly List<Entity> _destroyEntitiesCache = new();

        // 对象的唯一id, 该id不会因为被实例化, 或者反序列化而发生改变, 是在该实体被Entity.Create()的时候, 开始记录下来并永久保存的
        // 直接目的是为了反序列化的循环引用, 目前MemoryPack对于循环引用的支持并不灵活易用, 所以, 现在想到一个方案, 就是保存一个唯一不变的id, 这样就可以在反序列化过后, 通过查找的方式,
        // 拉取这个对象的引用. 但如此代价过大, 且实现的过程逻辑不够合理, 所以暂时还不实现. 只能说目前Memory Pack对循环引用的支持还不够友好
        // 2023.8.28, 但现在发现有另一个地方可以用到这个唯一id
        [ShowInInspector, LabelText("UniqueID"), ReadOnly]
        [MemoryPackOrder(0)]
        [MemoryPackInclude]
        protected internal int uniqueId;

        [ShowInInspector, LabelText("InstanceID"), ReadOnly]
        [MemoryPackIgnore]
        internal int instanceId;

        [MemoryPackIgnore]
        public int UniqueId => this.uniqueId;

        [MemoryPackIgnore]
        public int InstanceId => this.instanceId;

        [MemoryPackIgnore]
        public bool IsDisposed => this.InstanceId == 0;

        /// 是否是通过序列化创建的
        [MemoryPackIgnore]
        public bool IsDeserialized => this.uniqueId != this.instanceId;

        [MemoryPackIgnore]
        internal bool imminentDispose; // 即将被销毁

        protected virtual void BeginInit() { }

        // 实现unity的那套instantiate有点麻烦, MemoryPack不好用, 麻烦
        private static Object Instantiate(Object original) {
            original.BeginInit();
            var bytes = SerializeHelper.SerializeOfMemoryPack(original);
            var o = SerializeHelper.DeserializeOfMemoryPack<Object>(bytes);
            return o;
        }

        public static T Instantiate<T>(T original, Entity parent = null) where T : Object {
            switch (original) {
                case Entity originalEntity: {
                    var obj = Instantiate((Object)originalEntity);
                    var t = (T)obj;
                    switch (t) {
                        case Entity entity: {
                            entity.ForeachSerialize(child => { child.PartialOnCreated(); });
                            entity.SetParent(parent);
                            entity.InitializeBySerialization();
                            break;
                        }
                    }

                    return t;
                }

                case Component originalComponent: {
                    throw new Exception("暂时不支持组件实例化"); // 因为需要能直接给实体AddComponent(Component component); 实例化组件才有意义, 但现在暂时不考虑这种添加组件方式
                }
            }

            throw new Exception("instantiate error");
        }

        public static void Destroy(Object obj) {
            if (obj.IsDisposed) return;
            switch (obj) {
                case Component component:
                    Destroy(component);
                    break;

                case Entity entity:
                    Destroy(entity);
                    break;

                case Scene scene:
                    Destroy(scene);
                    break;
            }
        }

        public static void Destroy(Component component) {
            if (component.imminentDispose | component.IsDisposed) return;
            var instanceId = component.instanceId;
            component.imminentDispose = true;
            component.InternalOnDisable();
            component.InternalOnDestroy();
            component.entity.InternalRemoveComponent(component);
            component.OnDestroyFinish();
            EventSystemManager.Instance.UnregisterInstanced(instanceId);
        }

        public static void Destroy(Entity entity) {
            InternalDestroy(entity, false);
        }

        public static void DestroyChildren(Entity entity) {
            if (entity.children == null) return;
            for (var i = entity.children.Count - 1; i >= 0; i--) {
                InternalDestroy(entity.children[i], false);
            }
        }

        // 自下而上, 依次销毁
        // 事件顺序: 被销毁物体的父物体OnChildRemove -> 被销毁物体的OnDisable -> 被销毁物体的OnDestroy -> 被销毁物体的OnComponentRemove -> 所有组件和物体instanceId被设为0, 代表
        // 真正被销毁 -> 从EventSystem中注销
        // 这种顺序保证了, 我们在销毁实体而触发的各种事件中, 依然能正常的对自身组件、父级、子集之类的进行操作.
        private static void InternalDestroy(Entity entity, bool internalInvoke) {
            if (entity.imminentDispose | entity.IsDisposed)
                return;

            entity.imminentDispose = true;

            // 销毁一个实体时, 首先也只会把他自己的父级设置为null, 而不会把其他子实体的父级设置为null. 也就是说, 销毁一个实体, 并不会让实体解体, 不会改变其所有子实体的父子关系
            if (!internalInvoke) {
                entity.SetParent(null);
                _destroyComponentsCache.Clear();
                _destroyEntitiesCache.Clear();
            }

            var children = entity.children;
            if (children != null) {
                // 先从子物体开始销毁
                for (int i = 0, len = children.Count; i < len; i++) {
                    InternalDestroy(children[i], true);
                }
            }

            var components = entity.components;
            if (components != null) {
                _destroyComponentsCacheOnce.Clear();
                // 分层触发回调
                foreach (var kv in components) {
                    foreach (var component in kv.Value) {
                        if (component.imminentDispose | component.IsDisposed)
                            continue;

                        component.imminentDispose = true;
                        // 先遍历一遍, 触发Disable
                        component.InternalOnDisable();
                        _destroyComponentsCacheOnce.Add(component);
                        _destroyComponentsCache.Add(component);
                    }
                }

                for (int i = 0, len = _destroyComponentsCacheOnce.Count; i < len; i++) {
                    var component = _destroyComponentsCacheOnce[i];
                    // 再遍历一遍, 触发销毁
                    component.InternalOnDestroy();
                }

                for (int i = 0, len = _destroyComponentsCacheOnce.Count; i < len; i++) {
                    var component = _destroyComponentsCacheOnce[i];
                    // 最后再遍历一遍, 把所有组件从实体身上移除, 这样分层, 可以确保组件在触发销毁回调时, 依然可以正常的获取身上的其他组件
                    component.entity.InternalRemoveComponent(component);
                }
            }

            _destroyEntitiesCache.Add(entity);

            // 把自己和自己的所有子实体的所有组件, 都在递归中缓存下来, 再所有所有的事件都触发过后, 再统一对他们进行清空.
            if (!internalInvoke) {
                for (int i = 0, len = _destroyComponentsCache.Count; i < len; i++) {
                    var c = _destroyComponentsCache[i];
                    var cInstanceId = c.instanceId;
                    c.OnDestroyFinish();
                    EventSystemManager.Instance.UnregisterInstanced(cInstanceId);
                }

                for (int i = 0, len = _destroyEntitiesCache.Count; i < len; i++) {
                    var e = _destroyEntitiesCache[i];
                    var eInstanceId = e.instanceId;
                    e.OnDestroyFinish();
                    EventSystemManager.Instance.UnregisterInstanced(eInstanceId);
                }

                entity.OnDestroyFinish();
            }
        }

        public static void Destroy(Scene scene) {
            var sceneName = scene.sceneName;
            SceneManager.scenes.Remove(sceneName);

            foreach (var obj in EventSystemManager.Instance.GetAllInstance()) {
                if (obj is not Entity { parent: null } entity) continue;
                if (entity.scene != scene) continue;
                Destroy(entity);
            }
        }

        protected internal virtual void OnDestroyFinish() {
            this.uniqueId = 0;
            this.instanceId = 0;
        }
    }
}