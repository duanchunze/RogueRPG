using System;
using MemoryPack;

namespace Hsenl {
    [MemoryPackable()]
    public partial class IrregularCollider : Collider {
        private UnityEngine.Collider _collider;
        
        protected override UnityEngine.Collider UnityCollider => this._collider;

        protected override void OnAwake() {
            this._collider = this.GetMonoComponentInChildren<UnityEngine.Collider>();
            if (this._collider == null) {
                throw new NullReferenceException($"UnityEngine.Collider is not in {this.Name}");
            }

            if (!this.NonEvent) {
                var ptl = UnityPtlUnabridged.Get(this.Entity.GameObject);
                this.SetTrigger(ptl);
            }
        }
    }
}