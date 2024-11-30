using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class CapsuleCollider {
        private UnityEngine.CapsuleCollider _capsuleCollider;
        private UnityEngine.CapsuleCollider _CapsuleCollider => this._capsuleCollider ??= this.GetOrCreateUnityCollider<UnityEngine.CapsuleCollider>();

        protected override UnityEngine.Collider UnityCollider => this._CapsuleCollider;
        
        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._CapsuleCollider.center;
            set => this._CapsuleCollider.center = value;
        }

        [MemoryPackIgnore]
        public float Radius {
            get => this._CapsuleCollider.radius;
            set => this._CapsuleCollider.radius = value;
        }

        [MemoryPackIgnore]
        public float Height {
            get => this._CapsuleCollider.height;
            set => this._CapsuleCollider.height = value;
        }
    }
}