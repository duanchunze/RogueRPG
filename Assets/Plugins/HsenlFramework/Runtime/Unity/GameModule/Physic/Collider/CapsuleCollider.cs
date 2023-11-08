using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class CapsuleCollider {
        private UnityEngine.CapsuleCollider _capsuleCollider;
        
        protected override UnityEngine.Collider UnityCollider => this._capsuleCollider;
        
        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._capsuleCollider.center;
            set => this._capsuleCollider.center = value;
        }

        [MemoryPackIgnore]
        public float Radius {
            get => this._capsuleCollider.radius;
            set => this._capsuleCollider.radius = value;
        }

        [MemoryPackIgnore]
        public float Height {
            get => this._capsuleCollider.height;
            set => this._capsuleCollider.height = value;
        }

        protected override void OnConstruction() {
            if (this._capsuleCollider == null) {
                this._capsuleCollider = this.GetOrCreateUnityCollider<UnityEngine.CapsuleCollider>();
            }
        }
    }
}