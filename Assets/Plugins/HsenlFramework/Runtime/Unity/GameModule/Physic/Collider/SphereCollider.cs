using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class SphereCollider {
        private UnityEngine.SphereCollider _sphereCollider;

        protected override UnityEngine.Collider UnityCollider => this._sphereCollider;

        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._sphereCollider.center;
            set => this._sphereCollider.center = value;
        }

        [MemoryPackIgnore]
        public float Radius {
            get => this._sphereCollider.radius;
            set => this._sphereCollider.radius = value;
        }

        protected override void OnAwake() {
            if (this._sphereCollider == null) {
                this._sphereCollider = this.GetOrCreateUnityCollider<UnityEngine.SphereCollider>();
            }
        }
    }
}