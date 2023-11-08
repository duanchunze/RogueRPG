using System;
using UnityEngine;

namespace Hsenl {
    public class UnityPhysicsListener : MonoBehaviour {
        public Action<UnityEngine.Collider> OnTriggerEnterEvent;
        public Action<UnityEngine.Collider> OnTriggerStayEvent;
        public Action<UnityEngine.Collider> OnTriggerExitEvent;

        public Action<UnityEngine.Collision> OnCollisionEnterEvent;
        public Action<UnityEngine.Collision> OnCollisionStayEvent;
        public Action<UnityEngine.Collision> OnCollisionExitEvent;

        private UnityEngine.Rigidbody _rigidbody;
        private UnityEngine.Collider _collider;

        public static UnityPhysicsListener Get(GameObject go) {
            var result = go.GetComponent<UnityPhysicsListener>();
            if (result == null) {
                result = go.AddComponent<UnityPhysicsListener>();
                result._collider = result.GetComponent<UnityEngine.Collider>();
                result._rigidbody = result.GetComponent<UnityEngine.Rigidbody>();

                if (!result._collider && !result._rigidbody)
                    throw new Exception($"cant find collider or rigidbody on gameObject '{result.gameObject}'");
            }

            return result;
        }

        private void OnTriggerEnter(UnityEngine.Collider other) {
            this.OnTriggerEnterEvent?.Invoke(other);
        }

        private void OnTriggerStay(UnityEngine.Collider other) {
            this.OnTriggerStayEvent?.Invoke(other);
        }

        private void OnTriggerExit(UnityEngine.Collider other) {
            this.OnTriggerExitEvent?.Invoke(other);
        }

        private void OnCollisionEnter(UnityEngine.Collision other) {
            this.OnCollisionEnterEvent?.Invoke(other);
        }

        private void OnCollisionStay(UnityEngine.Collision other) {
            this.OnCollisionStayEvent?.Invoke(other);
        }

        private void OnCollisionExit(UnityEngine.Collision other) {
            this.OnCollisionExitEvent?.Invoke(other);
        }

        private void OnDestroy() {
            this.OnTriggerEnterEvent = null;
            this.OnTriggerStayEvent = null;
            this.OnTriggerExitEvent = null;
            this.OnCollisionEnterEvent = null;
            this.OnCollisionStayEvent = null;
            this.OnCollisionExitEvent = null;
        }

        public virtual void SetIncludeLayers(int layers) {
            if (this._collider) {
                this._collider.includeLayers = layers;
                return;
            }

            if (this._rigidbody)
                this._rigidbody.includeLayers = layers;
        }

        public virtual void SetExcludeLayers(int layers) {
            if (this._collider) {
                this._collider.excludeLayers = layers;
                return;
            }

            if (this._rigidbody)
                this._rigidbody.excludeLayers = layers;
        }
    }
}