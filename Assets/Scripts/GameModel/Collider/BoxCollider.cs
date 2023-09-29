using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class BoxCollider : Collider {
        private UnityEngine.BoxCollider _boxCollider;
        
        protected override UnityEngine.Collider UnityCollider => this._boxCollider;

        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._boxCollider.center;
            set => this._boxCollider.center = value;
        }

        [MemoryPackIgnore]
        public Vector3 Size {
            get => this._boxCollider.size;
            set => this._boxCollider.size = value;
        }

        [MemoryPackIgnore]
        public bool IsTrigger {
            get => this._boxCollider.isTrigger;
            set => this._boxCollider.isTrigger = value;
        }

        protected override void OnConstruction() {
            if (this._boxCollider == null) {
                this._boxCollider = this.Init<UnityEngine.BoxCollider>();
            }
        }
    }
}