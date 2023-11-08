using UnityEngine;

namespace Hsenl.CrossCombiner {
    [CombinerOptions(crossMaximumLayer = int.MaxValue)]
    public class BoxColliderRigidbodyCrossCombiner : CrossCombiner<BoxCollider, Rigidbody> {
        protected override void OnCombin(BoxCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(BoxCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }

    [CombinerOptions(crossMaximumLayer = int.MaxValue)]
    public class SphereColliderRigidbodyCrossCombiner : CrossCombiner<SphereCollider, Rigidbody> {
        protected override void OnCombin(SphereCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(SphereCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }

    [CombinerOptions(crossMaximumLayer = int.MaxValue)]
    public class CapsuleColliderRigidbodyCrossCombiner : CrossCombiner<CapsuleCollider, Rigidbody> {
        protected override void OnCombin(CapsuleCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(CapsuleCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }
}