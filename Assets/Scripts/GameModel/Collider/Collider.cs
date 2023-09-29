using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable(GenerateType.NoGenerate)]
    public abstract partial class Collider : Unbodied {
        [MemoryPackInclude]
        public bool NonEvent; // 只提供一个Collider, 但不会触发事件

        [MemoryPackIgnore]
        private Action<Collider> _onTriggerEnter;

        [MemoryPackIgnore]
        private Action<Collider> _onTriggerExit;

        [MemoryPackIgnore]
        private UnityPtlUnabridged triggerListener;

        [MemoryPackIgnore]
        protected abstract UnityEngine.Collider UnityCollider { get; }

        public void AddTriggerEnterListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't add listening");
            this._onTriggerEnter += action;
        }

        public void AddTriggerExitListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't add listening");
            this._onTriggerExit += action;
        }

        public void SetTriggerEnterListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't set listening");
            this._onTriggerEnter = action;
        }

        public void SetTriggerExitListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't set listening");
            this._onTriggerExit = action;
        }

        public void RemoveTriggerEnterListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't remove listening");
            this._onTriggerEnter -= action;
        }

        public void RemoveTriggerExitListening(Action<Collider> action) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't remove listening");
            this._onTriggerExit -= action;
        }

        public void ClearTriggerEnterListening() {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't clear listening");
            this._onTriggerEnter = null;
        }

        public void ClearTriggerExitListening() {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't clear listening");
            this._onTriggerExit = null;
        }

        public void SetTrigger(UnityPtlUnabridged listener) {
            if (this.NonEvent)
                throw new InvalidOperationException("collider is NonEvent model, so can't set trigger listener");

            if (this.triggerListener != null) {
                this.triggerListener.TriggerEnterEvent -= this.OnTriggerEnter;
                this.triggerListener.TriggerExitEvent -= this.OnTriggerExit;
            }

            this.triggerListener = listener;

            this.triggerListener.TriggerEnterEvent += this.OnTriggerEnter;
            this.triggerListener.TriggerExitEvent += this.OnTriggerExit;
        }

        private void OnTriggerEnter(UnityEngine.Collider other) {
            if (this._onTriggerEnter == null) return;
            var collider = other.gameObject.GetFrameworkComponent<Collider>();
            if (collider == null) return;
            this._onTriggerEnter.Invoke(collider);
        }

        private void OnTriggerExit(UnityEngine.Collider other) {
            if (this._onTriggerExit == null) return;
            var collider = other.gameObject.GetFrameworkComponent<Collider>();
            if (collider == null) return;
            this._onTriggerExit.Invoke(collider);
        }

        // 初始化unity的collider, 因为这里实例使用的还是unity的物理系统
        protected virtual T Init<T>() where T : UnityEngine.Collider {
            if (!this.Enable) {
                this.Entity.GameObjectEventLocked = true;
                this.Entity.GameObject.SetActive(false);
            }

            var collider = this.GetMonoComponentInChildren<T>();
            if (collider == null) {
                // throw new NullReferenceException($"UnityEngine.SphereCollider is not in {this.Name}");
                collider = this.Entity.GameObject.AddComponent<T>();
                collider.isTrigger = true;

                if (!this.NonEvent) {
                    var rigi = this.Entity.GameObject.AddComponent<Rigidbody>();
                    rigi.isKinematic = true;
                }
            }

            if (!this.Enable) {
                collider.enabled = false;
                this.Entity.GameObject.SetActive(true);
                this.Entity.GameObjectEventLocked = false;
            }

            if (!this.NonEvent) {
                var ptl = UnityPtlUnabridged.Get(this.Entity.GameObject);
                this.SetTrigger(ptl);
            }

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
        }

        public virtual void SetIncludeLayers(int layers) {
            this.UnityCollider.includeLayers = layers;
        }

        public virtual void SetExcludeLayers(int layers) {
            this.UnityCollider.excludeLayers = layers;
        }
    }
}