using FixedMath;

namespace Hsenl {
    public static class FVector3Extension {
        public static UnityEngine.Vector2 ToUnityVector2(this FVector2 self) {
            return new UnityEngine.Vector2(self.x, self.y);
        }

        public static FVector2 ToFVector2(this UnityEngine.Vector2 self) {
            return new FVector2(self.x, self.y);
        }
        
        public static UnityEngine.Vector3 ToUnityVector3(this FVector3 self) {
            return new UnityEngine.Vector3(self.x, self.y, self.z);
        }

        public static FVector3 ToFVector3(this UnityEngine.Vector3 self) {
            return new FVector3(self.x, self.y, self.z);
        }
        
        public static UnityEngine.Vector4 ToUnityVector4(this FVector4 self) {
            return new UnityEngine.Vector4(self.x, self.y, self.z, self.w);
        }

        public static FVector4 ToFVector4(this UnityEngine.Vector4 self) {
            return new FVector4(self.x, self.y, self.z, self.w);
        }
        
        public static UnityEngine.Quaternion ToUnityQuaternion(this FQuaternion self) {
            return new UnityEngine.Quaternion(self.x, self.y, self.z, self.w);
        }

        public static FQuaternion ToFQuaternion(this UnityEngine.Quaternion self) {
            return new FQuaternion(self.x, self.y, self.z, self.w);
        }
    }
}