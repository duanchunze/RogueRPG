using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using MemoryPack;
#if UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace Hsenl {
    [FrameworkMember]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Object {
        private static readonly HBuffer _buffer = new();

        // 对象的唯一id, 该id不会因为被实例化, 或者反序列化而发生改变, 是在该实体被Entity.Create()的时候, 开始记录下来并永久保存的
        // 直接目的是为了反序列化的循环引用, 目前MemoryPack对于循环引用的支持并不灵活易用, 所以, 现在想到一个方案, 就是保存一个唯一不变的id, 这样就可以在反序列化过后, 通过查找的方式,
        // 拉取这个对象的引用. 但如此代价过大, 且实现的过程逻辑不够合理, 所以暂时还不实现. 只能说目前Memory Pack对循环引用的支持还不够友好
        // 2023.8.28, 但现在发现有另一个地方可以用到这个唯一id
#if UNITY_EDITOR
        [ShowInInspector, LabelText("UniqueID"), ReadOnly]
#endif
        [MemoryPackOrder(0)]
        [MemoryPackInclude]
        internal int uniqueId;

#if UNITY_EDITOR
        [ShowInInspector, LabelText("InstanceID"), ReadOnly]
#endif
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
        internal bool disposing; // 正在销毁中

        protected virtual void BeginInit() { }

        // 实现unity的那套instantiate有点麻烦, MemoryPack不好用, 麻烦
        private static Object Instantiate(Object original) {
            original.BeginInit();
            _buffer.Seek(0, SeekOrigin.Begin);
            SerializeHelper.SerializeOfMemoryPack(_buffer, original);
            var o = SerializeHelper.DeserializeOfMemoryPack<Object>(_buffer.AsSpan(0, _buffer.Position));
            return o;
        }

        public static T Instantiate<T>(T original, Entity parent = null) where T : Object {
            switch (original) {
                case Entity originalEntity: {
                    var obj = Instantiate((Object)originalEntity);
                    var t = (T)obj;
                    switch (t) {
                        case Entity entity: {
// #if UNITY_5_3_OR_NEWER
//                             Foreach(entity);
//                             void Foreach(Entity e) {
//                                 foreach (var child in e.ForeachSerializeChildren()) {
//                                     child.PartialOnCreated();
//                                     Foreach(child);
//                                 }
//                             }
// #endif
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
            if (component.disposing | component.IsDisposed) return;
            var instanceId = component.instanceId;
            component.disposing = true;
            component.InternalOnDisable();
            component.InternalOnDestroy();
            component.entity.InternalRemoveComponent(component);
            component.Dispose();
            EventSystemManager.Instance.UnregisterInstanced(instanceId);
        }

        public static void Destroy(Entity entity) {
            using var componentsCache = ListComponent<Component>.Rent();
            using var entitiesCache = ListComponent<Entity>.Rent();
            InternalDestroy(entity, false, componentsCache, entitiesCache);
        }

        public static void DestroyChildren(Entity entity) {
            if (entity.children == null) return;
            for (var i = entity.children.Count - 1; i >= 0; i--) {
                using var componentsCache = ListComponent<Component>.Rent();
                using var entitiesCache = ListComponent<Entity>.Rent();
                InternalDestroy(entity.children[i], false, componentsCache, entitiesCache);
            }
        }

        // 自下而上, 依次销毁
        // 事件顺序: 被销毁物体的父物体OnChildRemove -> 被销毁物体的OnDisable -> 被销毁物体的OnDestroy -> 被销毁物体的OnComponentRemove -> 所有组件和物体instanceId被设为0, 代表
        // 真正被销毁 -> 从EventSystem中注销
        // 这种顺序保证了, 我们在销毁实体而触发的各种事件中, 依然能正常的对自身组件、父级、子集之类的进行操作.
        private static void InternalDestroy(Entity entity, bool internalInvoke, List<Component> componentsCache, List<Entity> entitiesCache) {
            if (entity.disposing || entity.IsDisposed)
                return;

            entity.disposing = true;

            var children = entity.children;
            if (children != null) {
                // 先从子物体开始销毁
                for (int i = 0, len = children.Count; i < len; i++) {
                    InternalDestroy(children[i], true, componentsCache, entitiesCache);
                }
            }

            var components = entity.components;
            if (components != null) {
                // 分层触发回调
                foreach (var kv in components) {
                    var comps = kv.Value;
                    for (int i = 0, len = comps.Count; i < len; i++) {
                        var c = comps[i];
                        if (c.disposing || c.IsDisposed)
                            continue;

                        c.disposing = true;
                        componentsCache.Add(c);
                    }
                }
            }

            entitiesCache.Add(entity);

            // 把自己和自己的所有子实体的所有组件, 都在递归中缓存下来, 再所有所有的事件都触发过后, 再统一对他们进行清空.
            if (!internalInvoke) {
                for (int i = 0, len = componentsCache.Count; i < len; i++) {
                    var c = componentsCache[i];
                    // 先遍历一遍, 触发Disable
                    c.InternalOnDisable();
                }

                for (int i = 0, len = componentsCache.Count; i < len; i++) {
                    var c = componentsCache[i];
                    // 再遍历一遍, 触发销毁
                    c.InternalOnDestroy();
                }

                // 事件都触发后, 再移除父级和所属场景
                var prevParent = entity.parent;
                entity.SetParentInternal(null, false); // 已经要销毁了, 就不再设置场景了
                entity.SetSceneInternal(null, prevParent, null);

                // 实体已经被销毁了, 也不必再触发组件移除事件了, 也就不用一个个的移除了, 等后面Dispose的时候, 一并清空就行
                // for (int i = 0, len = componentsCache.Count; i < len; i++) {
                //     var c = componentsCache[i];
                //     // 最后再遍历一遍, 把所有组件从实体身上移除, 这样分层, 可以确保组件在触发销毁回调时, 依然可以正常的获取身上的其他组件
                //     c.entity.InternalRemoveComponent(c);
                // }

                for (int i = 0, len = componentsCache.Count; i < len; i++) {
                    var c = componentsCache[i];
                    var cInstanceId = c.instanceId;
                    c.Dispose();
                    EventSystemManager.Instance.UnregisterInstanced(cInstanceId);
                }

                for (int i = 0, len = entitiesCache.Count; i < len; i++) {
                    var e = entitiesCache[i];
                    var eInstanceId = e.instanceId;
                    e.Dispose();
                    EventSystemManager.Instance.UnregisterInstanced(eInstanceId);
                }
            }
        }

        public static void Destroy(Scene scene) {
            var rootEntities = scene.RootEntities;
            while (rootEntities.Count != 0) {
                var entity = rootEntities[rootEntities.Count - 1];
                Destroy(entity);
            }

            SceneManager.scenes.Remove(scene.sceneName);
            scene.Dispose();
        }

        internal virtual void Dispose() {
            this.uniqueId = 0;
            this.instanceId = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void CheckDisposedException(string message) {
            if (this.IsDisposed)
                throw new Exception($"{message} {this}");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool CheckDisposingLog(string message) {
            if (this.disposing) {
                Log.Debug($"{message} {this}");
                return true;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void CheckDisposingException(string message) {
            if (this.disposing)
                throw new Exception($"{message} {this}");
        }
    }
}