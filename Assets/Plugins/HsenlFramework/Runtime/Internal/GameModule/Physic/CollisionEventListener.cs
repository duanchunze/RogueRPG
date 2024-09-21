using System;
using MemoryPack;

namespace Hsenl {
    [Serializable]
    [MemoryPackable()]
    public partial class CollisionEventListener : Unbodied {
        [MemoryPackIgnore]
        public Action<Collider> onTriggerEnter;

        [MemoryPackIgnore]
        public Action<Collider> onTriggerStay;

        [MemoryPackIgnore]
        public Action<Collider> onTriggerExit;

        [MemoryPackIgnore]
        public Action<Collider> onCollisionEnter;

        [MemoryPackIgnore]
        public Action<Collider> onCollisionStay;

        [MemoryPackIgnore]
        public Action<Collider> onCollisionExit;

        public static CollisionEventListener Get(Entity entity) {
            var result = entity.GetComponent<CollisionEventListener>();
            if (result == null) {
                result = entity.AddComponent<CollisionEventListener>();
            }

            return result;
        }
        
        internal override void OnDisposedInternal() {
            this.Clear();
        }

        public void Clear() {
            this.onTriggerEnter = null;
            this.onTriggerStay = null;
            this.onTriggerExit = null;
            this.onCollisionEnter = null;
            this.onCollisionStay = null;
            this.onCollisionExit = null;
        }
    }
}