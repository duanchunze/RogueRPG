using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class Entity : IGameObjectReference {
        [MemoryPackIgnore]
        public GameObject GameObject { get; private set; }

        [MemoryPackIgnore]
        public UnityEngine.Transform UnityTransform => this.GameObject.transform;

        void IGameObjectReference.SetUnityReference(GameObject reference) {
            this.GameObject = reference;
            if (reference == null)
                return;
            ((IEntityReference)reference.GetOrAddComponent<EntityReference>()).SetFrameworkReference(this);
        }

        private Entity(GameObject gameObject) {
            this.active = gameObject.activeSelf;
            this.name = gameObject.name;
            this.instanceId = Guid.NewGuid().GetHashCode();
            this.uniqueId = this.instanceId;
            this.imminentDispose = false;
            EventSystemManager.Instance.RegisterInstanced(this);
            ((IGameObjectReference)this).SetUnityReference(gameObject);
        }

        public static Entity Create(GameObject gameObject) {
            if (gameObject.GetComponent<EntityReference>() != null) throw new Exception("go entity referece is already exist");

            var entity = new Entity(gameObject);
            entity.transform = entity.AddComponent<Transform>();
            Entity parent = null;
            var monoParent = gameObject.transform.parent;
            if (monoParent) {
                // 通过gameObject来创建实体的时候, 如果gameObject有父级但却不是一个混合实体(unity的gameObject和框架的entity结合的产物), 则自动帮父级转换为混合实体
                parent = monoParent.gameObject.GetOrCreateEntityReference().Entity;
            }

            entity.SetParent(parent);
            return entity;
        }

        public void DontDestroyOnLoadWithUnity() {
            SceneManager.MoveEntityToSceneWithUnity(this, SceneManager.dontDestroyScene);
        }

        internal partial void PartialOnCreated() {
            var gameObject = new GameObject(this.Name);
            gameObject.SetActive(this.active);
            ((IGameObjectReference)this).SetUnityReference(gameObject);
        }

        internal void SetGameObject(GameObject gameObject) {
            if (this.GameObject != null) return;
            ((IGameObjectReference)this).SetUnityReference(gameObject);
        }

        internal partial void PartialOnParentChanged() {
            if (this.GameObject == null) return;
            if (this.parent == null) {
                this.GameObject.transform.SetParent(null);
                return;
            }

            var unityParent = this.parent.GameObject.transform;
            this.GameObject.transform.SetParent(unityParent, false);
        }

        internal partial void PartialOnActiveSelfChanged(bool act) {
            if (this.GameObject == null)
                return;

            this.GameObject.SetActive(act);
        }

        internal partial void PartialOnComponentAdd(Component component) {
            if (Framework.Instance.DisplayMono) {
                var type = MonoComponentManager.GetMonoComponentType(component.GetType());
                if (component.enable == false) {
                    // 这里做了一个处理, 因为Hsenl是支持添加一个默认enable为false的组件的, 但unity不支持, 所以这里使用了这个方案, 就是把go先关闭, 然后再添加mono组件, 
                    // 如此就能实现添加一个enable为false的mono组件
                    this.GameObject.SetActive(false);
                }

                var mono = (MonoBehaviour)this.GameObject.GetOrAddComponent(type);
                mono.enabled = component.enable;

                this.GameObject.SetActive(this.active);

                ((IMonoBehaviourReference)component).SetUnityReference(mono);
                ((IHsenlComponentReference)mono).SetFrameworkReference(component);
            }
        }

        internal partial void PartialOnDestroy() {
            if (this.GameObject != null) {
                var go = this.GameObject;
                go.GetComponent<IEntityReference>().SetFrameworkReference(null);
                this.GameObject = null;
                UnityEngine.Object.Destroy(go);
            }
        }
    }
}