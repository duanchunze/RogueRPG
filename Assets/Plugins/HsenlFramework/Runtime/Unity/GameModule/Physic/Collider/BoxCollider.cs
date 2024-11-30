using System;
using MemoryPack;
using UnityEngine;

namespace Hsenl {
    public partial class BoxCollider {
        private UnityEngine.BoxCollider _boxCollider;
        private UnityEngine.BoxCollider _BoxCollider => this._boxCollider ??= this.GetOrCreateUnityCollider<UnityEngine.BoxCollider>();
        
        protected override UnityEngine.Collider UnityCollider => this._BoxCollider;

        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._BoxCollider.center;
            set => this._BoxCollider.center = value;
        }

        [MemoryPackIgnore]
        public Vector3 Size {
            get => this._BoxCollider.size;
            set => this._BoxCollider.size = value;
        }
    }
}