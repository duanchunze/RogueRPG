namespace Hsenl {
    public partial class CollisionEventListener {
        private UnityPhysicsListener _unityPhysicsListener;

        protected override void OnDeserializedOverall() {
            this._unityPhysicsListener = UnityPhysicsListener.Get(this.Entity.GameObject);
            this.InitListener(this._unityPhysicsListener);
        }

        protected override void OnAwake() {
            this._unityPhysicsListener = UnityPhysicsListener.Get(this.Entity.GameObject);
            this.InitListener(this._unityPhysicsListener);
        }

        private void InitListener(UnityPhysicsListener upl) {
            upl.OnTriggerEnterEvent = other => {
                if (this.onTriggerEnter == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onTriggerEnter.Invoke(collider);
            };
            upl.OnTriggerStayEvent = other => {
                if (this.onTriggerStay == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onTriggerStay.Invoke(collider);
            };
            upl.OnTriggerExitEvent = other => {
                if (this.onTriggerExit == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onTriggerExit.Invoke(collider);
            };
            upl.OnCollisionEnterEvent = other => {
                if (this.onCollisionEnter == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onCollisionEnter.Invoke(collider);
            };
            upl.OnCollisionStayEvent = other => {
                if (this.onCollisionStay == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onCollisionStay.Invoke(collider);
            };
            upl.OnCollisionExitEvent = other => {
                if (this.onCollisionExit == null)
                    return;

                var collider = other.gameObject.GetFrameworkComponent<Collider>();
                if (collider == null)
                    return;

                this.onCollisionExit.Invoke(collider);
            };
        }
        
        public virtual void SetLayer(int layer) {
            this._unityPhysicsListener.gameObject.layer = layer;
            this._unityPhysicsListener.transform.ForeachChildren(child => { child.gameObject.layer = layer; });
        }

        public virtual void SetIncludeLayers(int layers) {
            this._unityPhysicsListener.SetIncludeLayers(layers);
        }

        public virtual void SetExcludeLayers(int layers) {
            this._unityPhysicsListener.SetExcludeLayers(layers);
        }
    }
}