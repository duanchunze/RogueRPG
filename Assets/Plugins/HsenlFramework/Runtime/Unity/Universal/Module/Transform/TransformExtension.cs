using UnityEngine;

namespace Hsenl {
    public static class TransformExtension {
        public static void NormalTransfrom(this Transform self) {
            self.LocalPosition = Vector3.zero;
            self.LocalRotation = Quaternion.identity;
            self.LocalScale = Vector3.one;
        }
    }
}