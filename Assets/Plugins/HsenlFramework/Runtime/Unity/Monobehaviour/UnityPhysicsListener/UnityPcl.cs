using System;
using UnityEngine;

namespace Hsenl {
    public class UnityPcl : UnityPhysicsListener<UnityPcl> {
        public event Action<Collision> CollisionEnterEvent;
        public event Action<Collision> CollisionExitEvent;

        private void OnCollisionEnter(Collision other) => this.CollisionEnterEvent?.Invoke(other);
        private void OnCollisionExit(Collision other) => this.CollisionExitEvent?.Invoke(other);
    }
}