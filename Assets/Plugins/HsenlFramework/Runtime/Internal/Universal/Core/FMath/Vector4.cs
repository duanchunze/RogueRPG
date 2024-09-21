using System;
using System.Runtime.InteropServices;

#if FIXED_MATH
using FLOAT = FixedMath.FMath.Fixp;
#else
using FLOAT = System.Single;
#endif

namespace Hsenl {
    [StructLayout(LayoutKind.Sequential)]
    [Serializable]
    public struct Vector4 : IEquatable<Vector4> {
        private static FLOAT ZeroEpsilonSq = Math.Epsilon;
        internal static Vector4 InternalZero = new Vector4(0, 0, 0, 0);

        public static readonly Vector4 Zero = new Vector4(0, 0, 0, 0);
        public static readonly Vector4 One = new Vector4(1, 1, 1, 1);
        public static readonly Vector4 MinValue = new Vector4(FLOAT.MinValue);
        public static readonly Vector4 MaxValue = new Vector4(FLOAT.MaxValue);

        public FLOAT x;
        public FLOAT y;
        public FLOAT z;
        public FLOAT w;

        public FLOAT sqrMagnitude => this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;

        public FLOAT magnitude {
            get {
                var num = this.sqrMagnitude;
                return Math.Sqrt(num);
            }
        }

        public Vector4 normalized {
            get {
                Vector4 result = new Vector4(this.x, this.y, this.z, this.w);
                result.Normalize();

                return result;
            }
        }

        public FLOAT this[int index] {
            get {
                return index switch {
                    0 => this.x,
                    1 => this.y,
                    2 => this.z,
                    3 => this.w,
                    _ => throw new IndexOutOfRangeException("vector idx invalid" + index)
                };
            }
            set {
                switch (index) {
                    case 0:
                        this.x = value;
                        break;
                    case 1:
                        this.y = value;
                        break;
                    case 2:
                        this.z = value;
                        break;
                    case 3:
                        this.w = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("vector idx invalid" + index);
                }
            }
        }

        public static Vector4 Abs(Vector4 other) {
            return new Vector4(Math.Abs(other.x), Math.Abs(other.y), Math.Abs(other.z),
                Math.Abs(other.z));
        }

        public static void Abs(ref Vector4 other, out Vector4 result) {
            result = new Vector4(Math.Abs(other.x), Math.Abs(other.y), Math.Abs(other.z),
                Math.Abs(other.z));
        }

        public static Vector4 ClampMagnitude(Vector4 vector, FLOAT min, FLOAT max) {
            var result = vector;
            var magnitude = vector.magnitude;
            var val = Math.Clamp(magnitude, min, max);
            if (!Math.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                result.x *= num;
                result.y *= num;
                result.z *= num;
                result.w *= num;
            }

            return result;
        }

        public static void ClampMagnitude(ref Vector4 vector, FLOAT min, FLOAT max) {
            var magnitude = vector.magnitude;
            var val = Math.Clamp(magnitude, min, max);
            if (!Math.CompareApproximate(val, magnitude)) {
                var num = val / magnitude;
                vector.x *= num;
                vector.y *= num;
                vector.z *= num;
                vector.w *= num;
            }
        }

        public static Vector4 Lerp(Vector4 from, Vector4 to, FLOAT percent) {
            percent = Math.Clamp(percent, 0, 1);
            return from + (to - from) * percent;
        }

        public static void Lerp(ref Vector4 from, ref Vector4 to, FLOAT percent, out Vector4 result) {
            percent = Math.Clamp(percent, 0, 1);
            result = from + (to - from) * percent;
        }

        public static Vector4 Scale(Vector4 v1, Vector4 v2) {
            Vector4 result;

            result.x = v1.x * v2.x;
            result.y = v1.y * v2.y;
            result.z = v1.z * v2.z;
            result.w = v1.w * v2.w;

            return result;
        }

        public static void Scale(ref Vector4 v1, ref Vector4 v2, out Vector4 result) {
            var x = v1.x * v2.x;
            var y = v1.y * v2.y;
            var z = v1.z * v2.z;
            var w = v1.w * v2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static Vector4 Min(Vector4 v1, Vector4 v2) {
            Min(ref v1, ref v2, out var result);
            return result;
        }

        public static void Min(ref Vector4 v1, ref Vector4 v2, out Vector4 result) {
            var x = (v1.x < v2.x) ? v1.x : v2.x;
            var y = (v1.y < v2.y) ? v1.y : v2.y;
            var z = (v1.z < v2.z) ? v1.z : v2.z;
            var w = (v1.w < v2.w) ? v1.w : v2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static Vector4 Max(Vector4 v1, Vector4 v2) {
            Max(ref v1, ref v2, out var result);
            return result;
        }

        public static void Max(ref Vector4 v1, ref Vector4 v2, out Vector4 result) {
            var x = (v1.x > v2.x) ? v1.x : v2.x;
            var y = (v1.y > v2.y) ? v1.y : v2.y;
            var z = (v1.z > v2.z) ? v1.z : v2.z;
            var w = (v1.w > v2.w) ? v1.w : v2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static FLOAT Distance(Vector4 v1, Vector4 v2) {
            return Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) +
                              (v1.z - v2.z) * (v1.z - v2.z) + (v1.w - v2.w) * (v1.w - v2.w));
        }

        public static void Distance(ref Vector4 v1, ref Vector4 v2, out FLOAT result) {
            result = Math.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) +
                                (v1.z - v2.z) * (v1.z - v2.z) + (v1.w - v2.w) * (v1.w - v2.w));
        }

        public static FLOAT Dot(Vector4 v1, Vector4 v2) {
            return ((v1.x * v2.x) + (v1.y * v2.y)) + (v1.z * v2.z) + (v1.w * v2.w);
        }

        public static void Dot(ref Vector4 v1, ref Vector4 v2, out FLOAT result) {
            result = ((v1.x * v2.x) + (v1.y * v2.y)) + (v1.z * v2.z) + (v1.w * v2.w);
        }

        public static Vector4 Normalize(Vector4 vector) {
            vector.Normalize();
            return vector;
        }

        public static void Normalize(ref Vector4 vector, out Vector4 result) {
            var num2 = (vector.x * vector.x) + (vector.y * vector.y) + (vector.z * vector.z) + (vector.w * vector.w);
            var num = 1 / Math.Sqrt(num2);
            var x = vector.x * num;
            var y = vector.y * num;
            var z = vector.z * num;
            var w = vector.w * num;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Swap(ref Vector4 vector1, ref Vector4 vector2) {
            var temp = vector1.x;
            vector1.x = vector2.x;
            vector2.x = temp;

            temp = vector1.y;
            vector1.y = vector2.y;
            vector2.y = temp;

            temp = vector1.z;
            vector1.z = vector2.z;
            vector2.z = temp;

            temp = vector1.w;
            vector1.w = vector2.w;
            vector2.w = temp;
        }

        public Vector4(FLOAT xyzw) {
            this.x = xyzw;
            this.y = xyzw;
            this.z = xyzw;
            this.w = xyzw;
        }

        public Vector4(FLOAT x, FLOAT y, FLOAT z, FLOAT w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void Scale(Vector4 other) {
            this.x = this.x * other.x;
            this.y = this.y * other.y;
            this.z = this.z * other.z;
            this.w = this.w * other.w;
        }

        public void Set(FLOAT x, FLOAT y, FLOAT z, FLOAT w) {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public void MakeZero() {
            this.x = 0;
            this.y = 0;
            this.z = 0;
            this.w = 0;
        }

        /// <summary>
        /// 是否为0（使用了Epsilon）
        /// </summary>
        /// <returns></returns>
        public bool IsZero() {
            return Math.CompareApproximateZero(this.sqrMagnitude);
        }

        public void Normalize() {
            var num2 = this.x * this.x + this.y * this.y + this.z * this.z + this.w * this.w;
            var num = 1 / Math.Sqrt(num2);
            this.x *= num;
            this.y *= num;
            this.z *= num;
            this.w *= num;
        }

        public void Negate() {
            this.x = -this.x;
            this.y = -this.y;
            this.z = -this.z;
            this.w = -this.w;
        }

        public Vector2 ToFVector2() {
            return new Vector2(this.x, this.y);
        }

        public Vector3 ToFVector3() {
            return new Vector3(this.x, this.y, this.z);
        }

        public bool Equals(Vector4 other) {
            return Math.CompareApproximate(this.x, other.x) &&
                   Math.CompareApproximate(this.y, other.y) &&
                   Math.CompareApproximate(this.z, other.z) &&
                   Math.CompareApproximate(this.w, other.w);
        }

        public override bool Equals(object obj) {
            if (!(obj is Vector4 other)) {
                return false;
            }

            return Math.CompareApproximate(this.x, other.x) &&
                   Math.CompareApproximate(this.y, other.y) &&
                   Math.CompareApproximate(this.z, other.z) &&
                   Math.CompareApproximate(this.w, other.w);
        }

        public override int GetHashCode() {
            return this.x.GetHashCode() ^ this.y.GetHashCode() ^ this.z.GetHashCode() ^ this.w.GetHashCode();
        }

#if FIXED_MATH
        public override string ToString() {
            return $"({x.AsFloat():f1}, {y.AsFloat():f1}, {z.AsFloat():f1}, {w.AsFloat():f1})";
        }
#else
        public override string ToString() {
            return $"({this.x:f1}, {this.y:f1}, {this.z:f1}, {this.w:f1})";
        }
#endif

        public static void Add(ref Vector4 value1, ref Vector4 value2, out Vector4 result) {
            var x = value1.x + value2.x;
            var y = value1.y + value2.y;
            var z = value1.z + value2.z;
            var w = value1.w + value2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Subtract(ref Vector4 value1, ref Vector4 value2, out Vector4 result) {
            var x = value1.x - value2.x;
            var y = value1.y - value2.y;
            var z = value1.z - value2.z;
            var w = value1.w - value2.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Multiply(ref Vector4 value1, FLOAT scaleFactor, out Vector4 result) {
            var x = value1.x * scaleFactor;
            var y = value1.y * scaleFactor;
            var z = value1.z * scaleFactor;
            var w = value1.w * scaleFactor;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Divide(ref Vector4 value1, FLOAT scaleFactor, out Vector4 result) {
            var x = value1.x / scaleFactor;
            var y = value1.y / scaleFactor;
            var z = value1.z / scaleFactor;
            var w = value1.w / scaleFactor;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static void Negate(ref Vector4 value, out Vector4 result) {
            var x = -value.x;
            var y = -value.y;
            var z = -value.z;
            var w = -value.w;

            result.x = x;
            result.y = y;
            result.z = z;
            result.w = w;
        }

        public static bool operator ==(Vector4 v1, Vector4 v2) {
            return Math.CompareApproximate(v1.x, v2.x) &&
                   Math.CompareApproximate(v1.y, v2.y) &&
                   Math.CompareApproximate(v1.z, v2.z) &&
                   Math.CompareApproximate(v1.w, v2.w);
        }

        public static bool operator !=(Vector4 v1, Vector4 v2) {
            if ((Math.CompareApproximate(v1.x, v2.x)) && (Math.CompareApproximate(v1.y, v2.y)) && (Math.CompareApproximate(v1.z, v2.z))) {
                return !Math.CompareApproximate(v1.w, v2.w);
            }

            return true;
        }

        public static Vector4 operator +(Vector4 v1, Vector4 v2) {
            Vector4 result;
            result.x = v1.x + v2.x;
            result.y = v1.y + v2.y;
            result.z = v1.z + v2.z;
            result.w = v1.w + v2.w;
            return result;
        }

        public static Vector4 operator -(Vector4 v1, Vector4 v2) {
            Vector4 result;
            result.x = v1.x - v2.x;
            result.y = v1.y - v2.y;
            result.z = v1.z - v2.z;
            result.w = v1.w - v2.w;
            return result;
        }

        public static Vector4 operator *(Vector4 v1, FLOAT v2) {
            Vector4 result;
            result.x = v1.x * v2;
            result.y = v1.y * v2;
            result.z = v1.z * v2;
            result.w = v1.w * v2;
            return result;
        }

        public static Vector4 operator *(FLOAT v1, Vector4 v2) {
            Vector4 result;
            result.x = v2.x * v1;
            result.y = v2.y * v1;
            result.z = v2.z * v1;
            result.w = v2.w * v1;
            return result;
        }

        public static Vector4 operator /(Vector4 v1, FLOAT v2) {
            Vector4 result;
            result.x = v1.x / v2;
            result.y = v1.y / v2;
            result.z = v1.z / v2;
            result.w = v1.w / v2;
            return result;
        }

        public static implicit operator Vector4(Vector3 v) => new Vector4(v.x, v.y, v.z, 0.0f);

        public static implicit operator Vector3(Vector4 v) => new Vector3(v.x, v.y, v.z);

        public static implicit operator Vector4(Vector2 v) => new Vector4(v.x, v.y, 0.0f, 0.0f);

        public static implicit operator Vector2(Vector4 v) => new Vector2(v.x, v.y);

#if UNITY_5_3_OR_NEWER
        public static implicit operator UnityEngine.Vector4(Vector4 v)
        {
            return new UnityEngine.Vector4(v.x, v.y, v.z, v.w);
        }

        public static implicit operator Vector4(UnityEngine.Vector4 v)
        {
            return new Vector4(v.x, v.y, v.z, v.w);
        }
#endif
    }
}