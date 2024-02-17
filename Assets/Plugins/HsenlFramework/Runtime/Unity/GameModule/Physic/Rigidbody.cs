using MemoryPack;

namespace Hsenl {
    public partial class Rigidbody {
        [MemoryPackIgnore]
        private UnityEngine.Rigidbody _rigidbody;

        public bool IsKinematic {
            get => this._rigidbody.isKinematic;
            set => this._rigidbody.isKinematic = value;
        }

        protected override void OnDeserialized() {
            this._rigidbody = this.GetMonoComponent<UnityEngine.Rigidbody>();
            if (this._rigidbody == null) {
                this._rigidbody = this.Entity.GameObject.AddComponent<UnityEngine.Rigidbody>();
            }
        }

        protected override void OnAwake() {
            this._rigidbody = this.GetMonoComponent<UnityEngine.Rigidbody>();
            if (this._rigidbody == null) {
                this._rigidbody = this.Entity.GameObject.AddComponent<UnityEngine.Rigidbody>();
            }
        }
        
        public virtual void SetLayer(int layer) {
            this._rigidbody.gameObject.layer = layer;
            this._rigidbody.transform.ForeachChildren(child => { child.gameObject.layer = layer; });
        }

        public virtual void SetIncludeLayers(int layers) {
            this._rigidbody.includeLayers = layers;
        }

        public virtual void SetExcludeLayers(int layers) {
            this._rigidbody.excludeLayers = layers;
        }
    }
}