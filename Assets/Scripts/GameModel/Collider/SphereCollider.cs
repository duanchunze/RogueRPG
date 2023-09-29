using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class SphereCollider : Collider {
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

        [MemoryPackIgnore]
        public bool IsTrigger {
            get => this._sphereCollider.isTrigger;
            set => this._sphereCollider.isTrigger = value;
        }

        protected override void OnConstruction() {
            if (this._sphereCollider == null) {
                this._sphereCollider = this.Init<UnityEngine.SphereCollider>();
            }
        }
    }
}