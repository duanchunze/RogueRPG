using System;
using UnityEngine;

namespace Hsenl {
    public class UnityPtlUnabridged : UnityPhysicsListener<UnityPtlUnabridged> {
        public event Action<Collider> TriggerEnterEvent;
        public event Action<Collider> TriggerStayEvent;
        public event Action<Collider> TriggerExitEvent;

        private void OnTriggerEnter(Collider other) => this.TriggerEnterEvent?.Invoke(other);
        private void OnTriggerStay(Collider other) => this.TriggerStayEvent?.Invoke(other);
        private void OnTriggerExit(Collider other) => this.TriggerExitEvent?.Invoke(other);
    }
}