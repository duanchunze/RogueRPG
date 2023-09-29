using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [MemoryPackable()]
    public partial class CapsuleCollider : Collider {
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
        
        [MemoryPackIgnore]
        public bool IsTrigger {
            get => this._capsuleCollider.isTrigger;
            set => this._capsuleCollider.isTrigger = value;
        }
        
        protected override void OnConstruction() {
            if (this._capsuleCollider == null) {
                this._capsuleCollider = this.Init<UnityEngine.CapsuleCollider>();
            }
        }
    }
}