using System;
using Unity.Mathematics;

namespace Hsenl {
    // 挂载该组件可以捡起拾取物
    [Serializable]
    public class Picker : Unbodied {
        public float Radius {
            get => this._sphereCollider.Radius;
            set => this._sphereCollider.Radius = value;
        }

        public float3 Center {
            get => this._sphereCollider.Center;
            set => this._sphereCollider.Center = value;
        }

        private SphereCollider _sphereCollider;

        public Action<Pickable> onPickUp;

        protected override void OnAwake() {
            if (this._sphereCollider != null) {
                Object.Destroy(this._sphereCollider.Entity);
            }

            this._sphereCollider = ColliderFactory.CreateCollider<SphereCollider>("Piaker Collider", GameColliderPurpose.Picker);
            this._sphereCollider.SetParent(this.Entity);

            this._sphereCollider.AddTriggerEnterListening(this.OnTrigger);
        }

        protected override void OnDestroy() {
            this._sphereCollider?.RemoveTriggerEnterListening(this.OnTrigger);
        }

        private void OnTrigger(Collider collider) {
            var pickable = collider.Substantive.GetComponent<Pickable>();
            if (pickable == null) return;

            this.OnPickUp(pickable);
        }

        private void OnPickUp(Pickable pickable) {
            try {
                this.onPickUp?.Invoke(pickable);
            }
            catch (Exception e) {
                Log.Error(e);
            }
        }

        public void Pickup(Pickable pickable) {
            this.OnPickUp(pickable);
        }
    }
}