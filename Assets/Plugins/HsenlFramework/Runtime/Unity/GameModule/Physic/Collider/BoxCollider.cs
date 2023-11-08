using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class BoxCollider {
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

        protected override void OnConstruction() {
            if (this._boxCollider == null) {
                this._boxCollider = this.GetOrCreateUnityCollider<UnityEngine.BoxCollider>();
            }
        }
    }
}