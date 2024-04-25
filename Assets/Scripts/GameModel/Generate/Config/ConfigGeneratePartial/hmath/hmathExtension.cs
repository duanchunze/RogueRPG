namespace Hsenl {
    public static class hmathExtension {
        public static UnityEngine.Vector2 ToUnityVector2(this hmath.Vector2 self) {
            return new UnityEngine.Vector2(self.X, self.Y);
        }

        public static UnityEngine.Vector3 ToUnityVector3(this hmath.Vector3 self) {
            return new UnityEngine.Vector3(self.X, self.Y, self.Z);
        }

        public static UnityEngine.Vector4 ToUnityVector4(this hmath.Vector4 self) {
            return new UnityEngine.Vector4(self.X, self.Y, self.Z, self.W);
        }
    }
}