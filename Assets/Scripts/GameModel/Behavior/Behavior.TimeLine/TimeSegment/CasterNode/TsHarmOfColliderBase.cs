using System;
using MemoryPack;

namespace Hsenl {
    public abstract class TsHarmOfColliderBase<T> : TsHarm<T> where T : timeline.TsHarmInfo {
        protected Collider collider;
        protected float tsize;

        protected override void OnTimeSegmentOrigin() {
            this.collider = ColliderManager.Instance.Rent(this.GetColliderName(), active: false);
            this.collider.IsTrigger = true;
            this.collider.SetUsage(GameColliderPurpose.Detection);
            this.tsize = GameAlgorithm.MergeCalculateNumeric(this.numerators, NumericType.Tsize);

            var eventListener = CollisionEventListener.Get(this.collider.Entity);
            eventListener.onTriggerEnter = this.OnTriggerEnter;
        }

        protected override void OnTimeSegmentRunning() {
            if (this.collider != null) {
                this.UpdateTransform();
                this.collider.Entity.Active = true     ;
            }
        }

        protected override void OnTimeSegmentTerminate(bool timeout) {
            if (this.collider is { IsDisposing: false }) {
                ColliderManager.Instance.Return(this.collider);
            }
        }

        protected void UpdateTransform() {
            var tran = this.manager.Bodied.transform;
            var center = this.GetCenter();
            center = center * this.tsize * tran.Quaternion;
            this.collider.transform.Position = tran.Position + center;
            this.collider.transform.Quaternion = tran.Quaternion;
            var size = this.GetSize();
            var localScale = size * this.tsize;
            localScale.y = 0.1f;
            this.collider.transform.LocalScale = localScale;
        }

        protected abstract string GetColliderName();
        protected abstract Vector3 GetCenter();
        protected abstract Vector3 GetSize();
        protected abstract void OnTriggerEnter(Collider other);
    }
}