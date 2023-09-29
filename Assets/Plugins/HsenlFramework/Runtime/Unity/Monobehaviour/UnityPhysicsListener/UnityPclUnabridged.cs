using System;
using UnityEngine;

namespace Hsenl {
    public class UnityPclUnabridged : UnityPhysicsListener<UnityPclUnabridged> {
        public event Action<Collision> CollisionEnterEvent;
        public event Action<Collision> CollisionStayEvent;
        public event Action<Collision> CollisionExitEvent;

        private void OnCollisionEnter(Collision other) => this.CollisionEnterEvent?.Invoke(other);
        private void OnCollisionStay(Collision other) => this.CollisionStayEvent?.Invoke(other);
        private void OnCollisionExit(Collision other) => this.CollisionExitEvent?.Invoke(other);
    }
}