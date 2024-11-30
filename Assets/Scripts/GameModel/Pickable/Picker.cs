using System;
using MemoryPack;

namespace Hsenl {
    // 挂载该组件可以捡起拾取物
    [Serializable]
    [MemoryPackable]
    public partial class Picker : Unbodied {
        private SphereCollider _collider;

        [MemoryPackIgnore]
        public float Radius {
            get => this._collider.Radius;
            set => this._collider.Radius = value;
        }

        [MemoryPackIgnore]
        public Vector3 Center {
            get => this._collider.Center;
            set => this._collider.Center = value;
        }

        [MemoryPackIgnore]
        public Action<Pickable> onPickUp;

        protected override void OnAwake() {
            if (this._collider != null) {
                Object.Destroy(this._collider.Entity);
            }

            this._collider = ColliderManager.Instance.Rent<SphereCollider>("Picker Collider");
            this._collider.IsTrigger = true;
            this._collider.SetUsage(GameColliderPurpose.Picker);
            this._collider.SetParent(this.Entity, false);
            var listener = CollisionEventListener.Get(this._collider.Entity);

            listener.onTriggerEnter = this.OnTrigger;
        }

        private void OnTrigger(Collider collider) {
            var pickable = collider.Bodied.GetComponent<Pickable>();
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