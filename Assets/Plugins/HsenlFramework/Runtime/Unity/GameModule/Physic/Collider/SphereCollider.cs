using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class SphereCollider {
        private UnityEngine.SphereCollider _sphereCollider;
        private UnityEngine.SphereCollider _SphereCollider => this._sphereCollider ??= this.GetOrCreateUnityCollider<UnityEngine.SphereCollider>();

        protected override UnityEngine.Collider UnityCollider => this._SphereCollider;

        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._SphereCollider.center;
            set => this._SphereCollider.center = value;
        }

        [MemoryPackIgnore]
        public float Radius {
            get => this._SphereCollider.radius;
            set => this._SphereCollider.radius = value;
        }
    }
}