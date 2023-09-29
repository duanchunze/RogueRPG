using System;
using UnityEngine;

namespace Hsenl {
    public class UnityPtl : UnityPhysicsListener<UnityPtl> {
        public event Action<Collider> TriggerEnterEvent;
        public event Action<Collider> TriggerExitEvent;

        private void OnTriggerEnter(Collider other) => this.TriggerEnterEvent?.Invoke(other);
        private void OnTriggerExit(Collider other) => this.TriggerExitEvent?.Invoke(other);
    }
}