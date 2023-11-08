using Hsenl.CrossCombiner;

namespace Hsenl.MultiCombiner {
    [CombinerOverride(typeof(BoxColliderRigidbodyCrossCombiner))]
    public class BoxColliderRigidbodyMultiCombiner : MultiCombiner<BoxCollider, Rigidbody> {
        protected override void OnCombin(BoxCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(BoxCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }

    [CombinerOverride(typeof(SphereColliderRigidbodyCrossCombiner))]
    public class SphereColliderRigidbodyMultiCombiner : MultiCombiner<SphereCollider, Rigidbody> {
        protected override void OnCombin(SphereCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(SphereCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }

    [CombinerOverride(typeof(CapsuleColliderRigidbodyCrossCombiner))]
    public class CapsuleColliderRigidbodyMultiCombiner : MultiCombiner<CapsuleCollider, Rigidbody> {
        protected override void OnCombin(CapsuleCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = arg2;
        }

        protected override void OnDecombin(CapsuleCollider arg1, Rigidbody arg2) {
            arg1.Rigidbody = null;
        }
    }
}