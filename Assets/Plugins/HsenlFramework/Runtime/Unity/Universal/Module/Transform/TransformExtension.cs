using UnityEngine;

namespace Hsenl {
    public static class TransformExtension {
        public static void NormalTransfrom(this Transform self) {
            self.LocalPosition = Vector3.Zero;
            self.LocalRotation = Quaternion.Identity;
            self.LocalScale = Vector3.One;
        }
    }
}