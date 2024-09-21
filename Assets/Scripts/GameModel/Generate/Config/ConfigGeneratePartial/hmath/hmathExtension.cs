namespace Hsenl {
    public static class hmathExtension {
        public static Vector2 ToVector2(this hmath.Vector2 self) {
            return new UnityEngine.Vector2(self.X, self.Y);
        }

        public static Vector3 ToVector3(this hmath.Vector3 self) {
            return new Vector3(self.X, self.Y, self.Z);
        }

        public static Vector4 ToVector4(this hmath.Vector4 self) {
            return new UnityEngine.Vector4(self.X, self.Y, self.Z, self.W);
        }
    }
}