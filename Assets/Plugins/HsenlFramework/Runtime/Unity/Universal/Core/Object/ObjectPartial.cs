using System;
using System.Collections.Generic;

namespace Hsenl {
    public partial class Object {
        private static readonly Dictionary<int, UnityEngine.GameObject> _caches = new();

        public static T InstantiateWithUnity<T>(T original, Entity parent = null) where T : Object {
            switch (original) {
                case Entity oriEntity: {
                    _caches.Clear();
                    var gameObject = UnityEngine.Object.Instantiate(oriEntity.GameObject);
                    foreach (var entityReference in gameObject.GetComponentsInChildren<EntityReference>(true)) {
                        _caches.Add(entityReference.uniqueId, entityReference.gameObject);
                    }

                    var obj = Instantiate((Object)oriEntity);
                    var t = (T)obj;
                    if (t is Entity entity) {
                        entity.SetGameObject(_caches[entity.UniqueId]);
                        Foreach(entity);

                        void Foreach(Entity e) {
                            foreach (var child in e.ForeachSerializeChildren()) {
                                child.SetGameObject(_caches[child.UniqueId]);
                                Foreach(child);
                            }
                        }

                        entity.SetParent(parent);
                        entity.InitializeBySerialization();
                    }

                    _caches.Clear();
                    return t;
                }

                case Component originalComponent: {
                    throw new Exception("暂时不支持组件实例化"); // 因为需要能直接给实体AddComponent(Component component); 实例化组件才有意义, 但现在暂时不考虑这种添加组件方式
                }
            }

            throw new Exception("instantiate error");
        }
    }
}