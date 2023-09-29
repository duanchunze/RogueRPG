using System;
using Unity.Mathematics;

namespace Hsenl {
    // 身体碰撞器
    [Serializable]
    public class PhysicBody : Unbodied {
        public float Height {
            get => this._capsuleCollider.Height;
            set => this._capsuleCollider.Height = value;
        }

        public float Radius {
            get => this._capsuleCollider.Radius;
            set => this._capsuleCollider.Radius = value;
        }

        public float3 Center {
            get => this._capsuleCollider.Center;
            set => this._capsuleCollider.Center = value;
        }
        
        private CapsuleCollider _capsuleCollider;
        
        protected override void OnAwake() {
            if (this._capsuleCollider != null) {
                Object.Destroy(this._capsuleCollider.Entity);
            }

            this._capsuleCollider = ColliderFactory.CreateCollider<CapsuleCollider>("Body Collider", GameColliderPurpose.Body, true);
            this._capsuleCollider.SetParent(this.Entity);
        }
    }
}