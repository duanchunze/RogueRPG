using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public abstract partial class Collider {
        [MemoryPackIgnore]
        protected abstract UnityEngine.Collider UnityCollider { get; }

        [MemoryPackIgnore]
        public bool IsTrigger {
            get => this.UnityCollider.isTrigger;
            set => this.UnityCollider.isTrigger = value;
        }

        // 初始化unity的collider, 因为这里实例使用的还是unity的物理系统
        protected virtual T GetOrCreateUnityCollider<T>() where T : UnityEngine.Collider {
            if (!this.Enable) {
                this.Entity.GameObject.SetActive(false);
            }

            var collider = this.GetMonoComponent<T>();
            if (collider == null) {
                collider = this.Entity.GameObject.AddComponent<T>();
            }

            collider.enabled = this.Enable;

            this.Entity.GameObject.SetActive(this.Entity.Active);

            return collider;
        }

        protected override void OnEnable() {
            this.UnityCollider.enabled = true;
        }

        protected override void OnDisable() {
            if (this.Enable) return;
            this.UnityCollider.enabled = false;
        }

        public virtual void SetLayer(int layer) {
            this.UnityCollider.gameObject.layer = layer;
            this.UnityCollider.transform.ForeachAllChildren((child, l) => { child.gameObject.layer = l; }, layer);
        }

        public virtual void SetIncludeLayers(int layers) {
            this.UnityCollider.includeLayers = layers;
        }

        public virtual void SetExcludeLayers(int layers) {
            this.UnityCollider.excludeLayers = layers;
        }
    }
}