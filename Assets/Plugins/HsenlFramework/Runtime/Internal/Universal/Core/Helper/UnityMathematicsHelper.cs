using Unity.Mathematics;

namespace Hsenl {
    public static class UnityMathematicsHelper {
        private const float Deg2Rad = (math.PI / 180f);
        private const float Rad2deg = (180f / math.PI);

        public static float3 eulerAngle(this quaternion self) {
            var f = self.value;
            var x = f.x * 2;
            var y = f.y * 2;
            var z = f.z * 2;
            var xx = f.x * x;
            var yy = f.y * y;
            var zz = f.z * z;
            var xy = f.x * y;
            var xz = f.x * z;
            var yz = f.y * z;
            var wx = f.w * x;
            var wy = f.w * y;
            var wz = f.w * z;

            var m11 = 1 - (yy + zz);
            var m12 = xy + wz;
            var m13 = xz - wy;
            var m21 = xy - wz;
            var m22 = 1 - (xx + zz);
            var m23 = yz + wx;
            var m31 = xz + wy;
            var m32 = yz - wx;
            var m33 = 1 - (xx + yy);

            float3 result = default;
            if (m23 < 0.999F) // some fudge for imprecision
            {
                if (m23 > -0.999F) // some fudge for imprecision
                {
                    result.x = math.asin(-m23);
                    result.y = math.atan2(m13, m33);
                    result.z = math.atan2(m21, m22);
                    result *= Rad2deg;
                    MakePositive(ref result);
                }
                else {
                    // WARNING.  Not unique.  YA - ZA = atan2(r01,r00)
                    result.x = math.PI * 0.5F;
                    result.y = math.atan2(m12, m11);
                    result.z = 0.0F;
                    result *= Rad2deg;
                    MakePositive(ref result);
                }
            }
            else {
                // WARNING.  Not unique.  YA + ZA = atan2(-r01,r00)
                result.x = -math.PI * 0.5F;
                result.y = math.atan2(-m12, m11);
                result.z = 0.0F;
                result *= Rad2deg;
                MakePositive(ref result);
            }

            return result;
        }

        public static quaternion createQuaternion(float3 eulerAngle) {
            var yaw = eulerAngle.y;
            var pitch = eulerAngle.x;
            var roll = eulerAngle.z;

            var p = pitch * 0.5f * Deg2Rad;
            var y = yaw * 0.5f * Deg2Rad;
            var r = roll * 0.5f * Deg2Rad;

            var sp = math.sin(p);
            var cp = math.cos(p);
            var sy = math.sin(y);
            var cy = math.cos(y);
            var sr = math.sin(r);
            var cr = math.cos(r);

            quaternion result = default;
            result.value.x = ((cy * sp) * cr) + ((sy * cp) * sr);
            result.value.y = ((sy * cp) * cr) - ((cy * sp) * sr);
            result.value.z = ((cy * cp) * sr) - ((sy * sp) * cr);
            result.value.w = ((cy * cp) * cr) + ((sy * sp) * sr);

            return result;
        }

        public static float3 MakePositive(ref float3 euler) {
            if (euler.x < 0) {
                euler.x += 360;
            }
            else if (euler.x >= 360) {
                euler.x -= 360;
            }

            if (euler.y < 0) {
                euler.y += 360;
            }
            else if (euler.y >= 360) {
                euler.y -= 360;
            }

            if (euler.z < 0) {
                euler.z += 360;
            }
            else if (euler.z >= 360) {
                euler.z -= 360;
            }

            return euler;
        }
    }
}